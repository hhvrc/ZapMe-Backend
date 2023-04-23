using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account username
    /// </summary>
    /// <param name="s3Client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">New username</response>
    /// <response code="400">Error details</response>
    [HttpPut("pfp", Name = "UpdateProfilePicture")]
    [Produces(Application.Json)]
    [BinaryPayload(true, "image/png", "image/jpeg", "image/webp", "image/gif")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromServices] IImageManager imageManager, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        long? length = Request.ContentLength;
        if (length == null)
        {
            return CreateHttpError.Generic(StatusCodes.Status411LengthRequired, "Length is required", "Missing Content-Length header").ToActionResult();
        }
        var res = await imageManager.GetOrCreateAsync(Request.Body, cancellationToken);
        if (length < 0)
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Invalid Content-Length", "Invalid Content-Length header").ToActionResult();
        }
        if (length > 1_112_000)
        {
            return CreateHttpError.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1MB").ToActionResult();
        }

        using MemoryStream stream = new MemoryStream((int)length);

        OneOf<ImageUtils.ParseResult, ErrorDetails> result = await ImageUtils.ParseAndRewriteFromStreamAsync(Request.Body, stream, cancellationToken);
        if (result.TryPickT0(out ImageUtils.ParseResult imageInfo, out ErrorDetails errorDetails))
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Unsupported or invalid image", "We failed to parse your image, please upload a valid one").ToActionResult();
        }

        if (imageInfo.Width > 1024 || imageInfo.Height > 1024)
        {
            return CreateHttpError.Generic(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1024x1024").ToActionResult();
        }

        byte[] sha256 = HashingUtils.Sha256_Bytes(stream);

        ImageEntity imageEntity = new ImageEntity()
        {
            Height = imageInfo.Height,
            Width = imageInfo.Width,
            FrameCount = imageInfo.FrameCount,
            SizeBytes = (uint)length,
            Sha256 = Convert.ToHexString(sha256),
            HashPerceptual = imageInfo.Phash,
            PublicUrl 
            UploaderId = identity.UserId
        };

        // Start transaction
        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Create DB record
        await _dbContext.Images.AddAsync(imageEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _dbContext.Users.Where(u => u.Id == identity.UserId).ExecuteUpdateAsync(spc => spc.SetProperty(u => u.ProfilePictureId, _ => imageEntity.Id).SetProperty(u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);

        // Upload file
        await s3Client.PutObjectAsync(new()
        {
            BucketName = "zapme-public",
            Key = imageEntity.Id.ToString(),
            ChecksumSHA256 = Convert.ToBase64String(sha256),
            InputStream = stream,
        }, cancellationToken);

        // Commit transaction
        await transaction.CommitAsync(cancellationToken);

        // Success
        return Ok();
    }
}