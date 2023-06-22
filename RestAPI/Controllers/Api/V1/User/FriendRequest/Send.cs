using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

partial class UserController
{
    /// <summary>
    /// Send friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpPost("i/{userId}/friendrequest", Name = "SendFriendRequest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestSend([FromRoute] Guid userId, [FromServices] IUserManager userManager, CancellationToken cancellationToken)
    {
        Guid authenticatedUserId = User.GetUserId();

        if (authenticatedUserId == userId)
            return BadRequest();

        bool success = await userManager.CreateFriendRequestAsync(userId, authenticatedUserId, cancellationToken);
        if (!success)
            return HttpErrors.Generic(
                StatusCodes.Status404NotFound,
                "friendrequest_not_found",
                "Friend request not found",
                NotificationSeverityLevel.Warning,
                "Friend request not found"
               ).ToActionResult();

        // TODO: raise notification

        return Ok(userId);
    }
}