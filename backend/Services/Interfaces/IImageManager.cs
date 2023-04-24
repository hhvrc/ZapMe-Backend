using Amazon.S3;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IImageManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="imageStream"></param>
    /// <param name="imageHash">Sha-256 hash of image bytes</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="AmazonS3Exception"></exception>
    Task UploadToS3Async(Guid imageId, Stream imageStream, byte[] imageHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="AmazonS3Exception"></exception>
    Task DeleteFromS3Async(Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageStream"></param>
    /// <param name="imageSizeBytes"></param>
    /// <param name="sha256Hash"></param>
    /// <param name="uploaderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>ImageEntity or ErrorDetails(400/413)</returns>
    Task<OneOf<ImageEntity, ErrorDetails>> GetOrCreateRecordAsync(Stream imageStream, ulong imageSizeBytes, string? sha256Hash = null, Guid? uploaderId = null, CancellationToken cancellationToken = default);
}
