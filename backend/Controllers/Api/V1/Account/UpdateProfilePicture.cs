﻿using Microsoft.AspNetCore.Mvc;
using OneOf;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account profile picture
    /// </summary>
    /// <param name="sha256Hash">[Optional] Sha-256 hash of the image bytes to verify the integrity of the image server-side</param>
    /// <param name="imageManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="201">Info about uploaded image</response>
    /// <response code="400">Payload is unsupported/corrupted or the hash (if provided) does not match the payload</response>
    /// <response code="411">Length is required</response>
    /// <response code="413">Image dimensions or byte size is too large</response>
    [HttpPut("pfp", Name = "UpdateProfilePicture")]
    [BinaryPayload(true, "image/png", "image/jpeg", "image/webp", "image/gif")]
    [ProducesResponseType(typeof(UpdateProfilePictureOk), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status411LengthRequired)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    public async Task<IActionResult> UpdateProfilePicture([FromHeader(Name = "Hash-Sha256")] string? sha256Hash, [FromServices] IImageManager imageManager, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        long length = Request.ContentLength ?? -1;
        if (length is <= 0 or > Int32.MaxValue)
        {
            return CreateHttpError.Generic(StatusCodes.Status411LengthRequired, "Length is required", "Missing or invalid Content-Length header").ToActionResult();
        }

        string cfRegion = CountryRegionLookup.GetCloudflareRegion(this.GetCloudflareIPCountry());

        OneOf<ImageEntity, ErrorDetails> res = await imageManager.GetOrCreateRecordAsync(Request.Body, cfRegion, (int)length, sha256Hash, identity.UserId, cancellationToken);
        if (res.TryPickT1(out ErrorDetails error, out ImageEntity image))
        {
            return error.ToActionResult();
        }

        return Created(image.PublicUrl, new UpdateProfilePictureOk
        {
            ImageId = image.Id,
            ImageUrl = image.PublicUrl,
            ImageHash = image.Sha256
        });
    }
}