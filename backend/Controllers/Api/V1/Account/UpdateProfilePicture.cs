using Microsoft.AspNetCore.Mvc;
using OneOf;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account._Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account username
    /// </summary>
    /// <param name="imageManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">New username</response>
    /// <response code="400">Error details</response>
    [HttpPut("pfp", Name = "UpdateProfilePicture")]
    [Produces(Application.Json)]
    [BinaryPayload(true, "image/png", "image/jpeg", "image/webp", "image/gif")]
    [ProducesResponseType(typeof(UpdateProfilePictureResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfilePicture([FromServices] IImageManager imageManager, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        long? length = Request.ContentLength;
        if (length == null)
        {
            return CreateHttpError.Generic(StatusCodes.Status411LengthRequired, "Length is required", "Missing Content-Length header").ToActionResult();
        }
        OneOf<ImageEntity, ErrorDetails> res = await imageManager.GetOrCreateRecordAsync(Request.Body, (ulong)length, identity.UserId, cancellationToken);
        if (res.TryPickT1(out ErrorDetails error, out ImageEntity image))
        {
            return error.ToActionResult();
        }

        return Ok(new UpdateProfilePictureResult
        {
            ImageId = image.Id,
            ImageUrl = image.PublicUrl,
            ImageHash = image.Sha256
        });
    }
}