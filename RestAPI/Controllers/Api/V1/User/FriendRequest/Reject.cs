using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Threading;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Delete outgoing/Reject incoming friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpDelete("i/{userId}/friendrequest", Name = "DenyFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestDeny([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid authenticatedUserId = User.GetUserId();

        // You can't reject a friend request from yourself, that would be weird
        if (authenticatedUserId == userId)
            return BadRequest();

        bool success = await _userManager.DeleteFriendRequestAsync(authenticatedUserId, userId, cancellationToken);

        return success
            ? Ok()
            : HttpErrors.Generic(
                StatusCodes.Status404NotFound,
                "friendrequest_not_found",
                "Friend request not found",
                NotificationSeverityLevel.Warning,
                "Friend request not found"
              ).ToActionResult();
    }
}