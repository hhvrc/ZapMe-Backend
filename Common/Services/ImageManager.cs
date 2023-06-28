using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using OneOf;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Extensions;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums.Errors;
using ZapMe.Mappers;
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

    public async Task<OneOf<ImageEntity, ImageUploadError>> GetOrCreateRecordAsync(string imageUrl, string regionName, string? sha256Hash, Guid? uploaderId, CancellationToken cancellationToken)
    {
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(imageUrl, cancellationToken);
        long contentLength = response.Content.Headers.ContentLength ?? -1;
        if (contentLength is <= 0 or > Int32.MaxValue)
        {
            return ImageUploadError.PayloadSizeInvalid;
        }

        Stream imageStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await GetOrCreateRecordAsync(imageStream, regionName, (int)contentLength, null, null, cancellationToken);
    }

    public async Task<OneOf<ImageEntity, ImageUploadError>> GetOrCreateRecordAsync(Stream imageStream, string regionName, int imageSizeBytes, string? sha256Hash, Guid? uploaderId, CancellationToken cancellationToken)
    {
        if (imageSizeBytes > ImageConstants.MaxImageSize)
        {
            return ImageUploadError.PayloadSizeTooLarge;
        }

        // Create stream that can be read multiple times, imageSizeBytes is initially just a hint
        using MemoryStream memoryStream = imageSizeBytes > 0 ? new MemoryStream(imageSizeBytes) : new MemoryStream();

        // Parse image for metadata and rewrite to webp/gif
        var result = await ImageParsing.ParseAndRewriteFromStreamAsync(imageStream, memoryStream, cancellationToken);
        if (result.TryPickT1(out ImageParseError parseError, out ImageMetaData imageInfo))
        {
            return ImageParseErrorMapper.MapToUploadError(parseError);
        }
        long contentLength = memoryStream.Length;
        if (contentLength > ImageConstants.MaxImageSize)
        {
            return ImageUploadError.PayloadSizeTooLarge;
        }
        if (contentLength <= 0)
        {
            return ImageUploadError.PayloadSizeInvalid;
        }

        // Hash image data
        byte[] sha256 = HashingUtils.Sha256_Bytes(memoryStream);
        string sha256_hex = Convert.ToHexString(sha256);

        if (sha256Hash is not null && !sha256_hex.Equals(sha256Hash, StringComparison.OrdinalIgnoreCase))
        {
            return ImageUploadError.PayloadChecksumMismatch;
        }

        // Check if image already exists
        ImageEntity? image = await _dbContext
            .Images
            .FirstOrDefaultAsync(i => i.Sha256 == sha256_hex, cancellationToken);
        if (image is not null)
        {
            return image;
        }

        if (imageInfo.Width > 1024 || imageInfo.Height > 1024)
        {
            return ImageUploadError.ImageDimensionsTooLarge;
        }

        image = new ImageEntity()
        {
            Height = imageInfo.Height,
            Width = imageInfo.Width,
            FrameCount = imageInfo.FrameCount,
            SizeBytes = (uint)contentLength,
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

            // Create DB record
            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Upload to S3 bucket
            await UploadToS3Async(image.Id, memoryStream, imageInfo.MimeType, sha256, regionName, cancellationToken);
            uploaded = true;

            // Commit transaction
            if (transaction is not null)
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
