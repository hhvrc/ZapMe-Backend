using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public class ImageManager : IImageManager
{
    readonly ZapMeContext _dbContext;
    readonly IAmazonS3 _s3Client;
    readonly string _regionName;
    readonly string _bucketName;

    public ImageManager(ZapMeContext dbContext, IAmazonS3 s3Client, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _s3Client = s3Client;
        _regionName = configuration.GetOrThrow("AmazonAWS:Region");
        _bucketName = configuration.GetOrThrow("AmazonAWS:S3:PublicBucketName");
    }

    public async Task UploadToS3Async(Guid imageId, Stream imageStream, byte[] imageHash, CancellationToken cancellationToken = default)
    {
        await _s3Client.PutObjectAsync(new()
        {
            BucketName = _bucketName,
            Key = $"img_{imageId}",
            ChecksumSHA256 = Convert.ToBase64String(imageHash),
            InputStream = imageStream,
        }, cancellationToken);
    }

    public async Task DeleteFromS3Async(Guid imageId, CancellationToken cancellationToken = default)
    {
        await _s3Client.DeleteObjectAsync(new()
        {
            BucketName = _bucketName,
            Key = $"img_{imageId}",
        }, cancellationToken);
    }

    public async Task<OneOf<ImageEntity, ErrorDetails>> GetOrCreateRecordAsync(Stream imageStream, ulong imageSizeBytes, Guid? uploaderId, CancellationToken cancellationToken)
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
            S3BucketName = _bucketName,
            S3RegionName = _regionName,
            UploaderId = uploaderId
        };

        bool uploaded = false, transactionOk = false;
        try
        {
            // Start transaction
            using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            // Upload to S3 bucket
            await UploadToS3Async(id, memoryStream, sha256, cancellationToken);
            uploaded = true;

            // Create DB record
            await _dbContext.Images.AddAsync(image, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);
            transactionOk = true;
        }
        finally
        {
            // Attempt to clean up if partially successful
            if (uploaded && !transactionOk)
            {
                await DeleteFromS3Async(image.Id, cancellationToken);
            }
        }

        return image;
    }
}
