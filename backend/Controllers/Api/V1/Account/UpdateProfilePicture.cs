using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
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
    [Produces(Application.Json, Application.Xml)]
    [BinaryPayload(true, "image/png", "image/jpeg", "image/webp", "image/gif")]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromServices] IAmazonS3 s3Client, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        long? length = Request.ContentLength;
        if (length == null)
        {
            return this.Error(StatusCodes.Status411LengthRequired, "Length is required", "Missing Content-Length header");
        }
        if (length < 0)
        {
            return this.Error(StatusCodes.Status400BadRequest, "Invalid Content-Length", "Invalid Content-Length header");
        }
        if (length > 1_112_000)
        {
            return this.Error(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1MB");
        }

        string? contentType = Request.ContentType;
        if (contentType == null)
        {
            return BadRequest("Missing Content-Type header");
        }
        if (contentType is not ("image/png" or "image/jpeg" or "image/webp" or "image/gif"))
        {
            return this.Error(StatusCodes.Status415UnsupportedMediaType, "Unsupported media type", "Unsupported media type, only PNG, JPEG, WEBP and GIF are supported");
        }

        using MemoryStream stream = new MemoryStream((int)length);
        await Request.Body.CopyToAsync(stream, cancellationToken);

        ImageEntity imageEntity;
        try
        {
            ImageUtils.ParseResult parsed = await ImageUtils.ParseFromStreamAsync(stream);

            if (parsed.Width > 1024 || parsed.Height > 1024)
            {
                return this.Error(StatusCodes.Status413PayloadTooLarge, "Payload too large", "Image too large, max 1024x1024");
            }

            imageEntity = new ImageEntity()
            {
                Height = parsed.Height,
                Width = parsed.Width,
                SizeBytes = (uint)length,
                Sha256 = parsed.Hash,
                HashPerceptual = parsed.Phash,
                UploaderId = identity.UserId
            };
        }
        catch (Exception)
        {
            return BadRequest("Invalid image");
        }

        // Start transaction
        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Create DB record
        await _dbContext.Images.AddAsync(imageEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Upload file
        await s3Client.PutObjectAsync(new(){
            BucketName = "zapme-public",
            Key = Guid.NewGuid().ToString(),
            InputStream = stream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        }, cancellationToken);

        // Commit transaction
        await transaction.CommitAsync(cancellationToken);

        // Success
        return Ok(imageEntity);
    }
}