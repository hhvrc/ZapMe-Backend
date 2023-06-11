using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class ImageManager : IImageManager
{
    readonly DatabaseContext _dbContext;
    readonly ICloudflareR2Service _cloudflareR2Service;

    public ImageManager(DatabaseContext dbContext, ICloudflareR2Service cloudflareR2Service, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _cloudflareR2Service = cloudflareR2Service;
    }

    public Task UploadToS3Async(Guid imageId, Stream imageStream, string imageMimeType, byte[] imageHash, string regionName, CancellationToken cancellationToken)
    {
        return _cloudflareR2Service.UploadObjectAsync(new()
        {
            BucketName = $"zapme-public-{regionName}",
            Key = $"img_{imageId}",
            InputStream = imageStream,
            ContentType = imageMimeType,
            ChecksumSHA256 = Convert.ToBase64String(imageHash),
            DisablePayloadSigning = true, // TODO: remove when Cloudflare fixes their S3 implementation
        }, cancellationToken);
    }

    public Task DeleteFromS3Async(Guid imageId, string regionName, CancellationToken cancellationToken)
    {
        return _cloudflareR2Service.DeleteObjectAsync(new()
        {
            BucketName = $"zapme-public-{regionName}",
            Key = $"img_{imageId}",
        }, cancellationToken);
    }

    public async Task<OneOf<ImageEntity, ErrorDetails>> GetOrCreateRecordAsync(string imageUrl, string regionName, string? sha256Hash, Guid? uploaderId, CancellationToken cancellationToken)
    {
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(imageUrl, cancellationToken);
        long contentLength = response.Content.Headers.ContentLength ?? -1;
        if (contentLength is <= 0 or > Int32.MaxValue)
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Invalid image", "Image size is invalid");
        }

        Stream imageStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await GetOrCreateRecordAsync(imageStream, regionName, (int)contentLength, null, null, cancellationToken);
    }

    public async Task<OneOf<ImageEntity, ErrorDetails>> GetOrCreateRecordAsync(Stream imageStream, string regionName, int imageSizeBytes, string? sha256Hash, Guid? uploaderId, CancellationToken cancellationToken)
    {
        if (imageSizeBytes > 1_112_000) // TODO: remove magic number
        {
            return HttpErrors.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1MB");
        }

        // Create stream that can be read multiple times, imageSizeBytes is initially just a hint
        using MemoryStream memoryStream = imageSizeBytes > 0 ? new MemoryStream(imageSizeBytes) : new MemoryStream();

        // Parse image for metadata and rewrite to webp/gif
        OneOf<ImageUtils.ParseResult, ErrorDetails> result = await ImageUtils.ParseAndRewriteFromStreamAsync(imageStream, memoryStream, cancellationToken);
        if (result.TryPickT1(out ErrorDetails errorDetails, out ImageUtils.ParseResult imageInfo))
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Unsupported or invalid image", errorDetails.Detail);
        }

        // Hash image data
        byte[] sha256 = HashingUtils.Sha256_Bytes(memoryStream);
        string sha256_hex = Convert.ToHexString(sha256);

        if (sha256Hash != null && !sha256_hex.Equals(sha256Hash, StringComparison.OrdinalIgnoreCase))
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Checksum mismatch", "The provided checksum does not match the image data");
        }

        // Check if image already exists
        ImageEntity? image = await _dbContext.Images.SingleOrDefaultAsync(i => i.Sha256 == sha256_hex, cancellationToken);
        if (image != null)
        {
            return image;
        }

        if (imageInfo.Width > 1024 || imageInfo.Height > 1024)
        {
            return HttpErrors.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1024x1024");
        }

        Guid id = Guid.NewGuid();
        image = new ImageEntity()
        {
            Id = id,
            Height = imageInfo.Height,
            Width = imageInfo.Width,
            FrameCount = imageInfo.FrameCount,
            SizeBytes = (uint)memoryStream.Length, // TODO: maybe possible crash if image is too large (very unlikely though)
            MimeType = imageInfo.MimeType,
            Sha256 = sha256_hex,
            R2RegionName = regionName,
            UploaderId = uploaderId
        };

        bool uploaded = false, transactionOk = false;
        try
        {
            // Start transaction
            using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            // Upload to S3 bucket
            await UploadToS3Async(id, memoryStream, imageInfo.MimeType, sha256, regionName, cancellationToken);
            uploaded = true;

            // Create DB record
            await _dbContext.Images.AddAsync(image, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Commit transaction
            if (transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            transactionOk = true;
        }
        finally
        {
            // Attempt to clean up if partially successful
            if (uploaded && !transactionOk)
            {
                await DeleteFromS3Async(image.Id, regionName, cancellationToken);
            }
        }

        return image;
    }
}
