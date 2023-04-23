﻿using OneOf;
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
    Task UploadToS3Async(Guid imageId, Stream imageStream, byte[] imageHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteFromS3Async(Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageStream"></param>
    /// <param name="imageSizeBytes"></param>
    /// <param name="uploaderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<ImageEntity, ErrorDetails>> GetOrCreateRecordAsync(Stream imageStream, ulong imageSizeBytes, Guid? uploaderId = null, CancellationToken cancellationToken = default);
}
