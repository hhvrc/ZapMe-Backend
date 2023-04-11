using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Attributes;
using ZapMe.Controllers.Api.V1.Models;
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
    [AllowAnonymous]
    [HttpPut("pfp", Name = "UpdateProfilePicture")]
    [Produces(Application.Json, Application.Xml)]
    [BinaryPayload(true, "image/png", "image/jpeg", "image/webp", "image/gif")]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromServices] IAmazonS3 s3Client, CancellationToken cancellationToken)
    {
        long? length = Request.ContentLength;
        if (length == null)
        {
            return BadRequest();
        }
        if (length > 1_112_000)
        {
            return BadRequest();
        }

        string? contentType = Request.ContentType;
        if (contentType == null)
        {
            return BadRequest();
        }
        if (contentType is not ("image/png" or "image/jpeg" or "image/webp" or "image/gif"))
        {
            return BadRequest();
        }

        byte[] data = new byte[(int)length!];
        using MemoryStream ms = new(data);

        await Request.Body.CopyToAsync(ms, cancellationToken);

        PutObjectRequest request = new()
        {
            BucketName = "zapme-public",
            Key = Guid.NewGuid().ToString(),
            InputStream = ms,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await s3Client.PutObjectAsync(request, cancellationToken);
        return Ok($"File uploaded to S3 successfully!");
    }
}