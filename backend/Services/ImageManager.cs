using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class ImageManager : IImageManager
{
    readonly ZapMeContext _dbContext;
    readonly IAmazonS3 _s3Client;

    public ImageManager(ZapMeContext dbContext, IAmazonS3 s3Client, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _s3Client = s3Client;
    }

    public async Task UploadToS3Async(Guid imageId, Stream imageStream, byte[] imageHash, string regionName, CancellationToken cancellationToken)
    {
        await _s3Client.PutObjectAsync(new()
        {
            BucketName = $"zapme-public-{regionName}",
            Key = $"img_{imageId}",
            ChecksumSHA256 = Convert.ToBase64String(imageHash),
            InputStream = imageStream,
        }, cancellationToken);
    }

    public async Task DeleteFromS3Async(Guid imageId, string regionName, CancellationToken cancellationToken)
    {
        await _s3Client.DeleteObjectAsync(new()
        {
            BucketName = $"zapme-public-{regionName}",
            Key = $"img_{imageId}",
        }, cancellationToken);
    }

    public async Task<OneOf<ImageEntity, ErrorDetails>> GetOrCreateRecordAsync(Stream imageStream, ulong imageSizeBytes, string regionName, string? sha256Hash, Guid? uploaderId, CancellationToken cancellationToken)
    {
        if (imageSizeBytes < 0)
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Invalid Content-Length", "Invalid Content-Length header");
        }
        if (imageSizeBytes > 1_112_000) // TODO: remove magic number
        {
            return CreateHttpError.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1MB");
        }

        // Create stream that can be read multiple times
        using MemoryStream memoryStream = new MemoryStream((int)imageSizeBytes);

        // Parse image for metadata and rewrite to webp/gif
        OneOf<ImageUtils.ParseResult, ErrorDetails> result = await ImageUtils.ParseAndRewriteFromStreamAsync(imageStream, memoryStream, cancellationToken);
        if (result.TryPickT1(out ErrorDetails errorDetails, out ImageUtils.ParseResult imageInfo))
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Unsupported or invalid image", errorDetails.Detail);
        }

        // Hash image data
        byte[] sha256 = HashingUtils.Sha256_Bytes(memoryStream);
        string sha256_hex = Convert.ToHexString(sha256);

        if (sha256Hash != null && !sha256_hex.Equals(sha256Hash, StringComparison.OrdinalIgnoreCase))
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Checksum mismatch", "The provided checksum does not match the image data");
        }

        // Check if image already exists
        ImageEntity? image = await _dbContext.Images.SingleOrDefaultAsync(i => i.Sha256 == sha256_hex, cancellationToken);
        if (image != null)
        {
            return image;
        }

        if (imageInfo.Width > 1024 || imageInfo.Height > 1024)
        {
            return CreateHttpError.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1024x1024");
        }

        Guid id = Guid.NewGuid();
        image = new ImageEntity()
        {
            Id = id,
            Height = imageInfo.Height,
            Width = imageInfo.Width,
            FrameCount = imageInfo.FrameCount,
            SizeBytes = (uint)imageSizeBytes,
            Extension = imageInfo.Extension,
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
            await UploadToS3Async(id, memoryStream, sha256, regionName, cancellationToken);
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
