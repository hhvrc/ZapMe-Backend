using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Accept incoming friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpPut("i/{userId}/friendrequest", Name = "AcceptFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]        // Accepted
    [ProducesResponseType(StatusCodes.Status304NotModified)] // Already friends
    [ProducesResponseType(StatusCodes.Status404NotFound)]    // No friendrequest incoming
    public async Task<IActionResult> FriendRequestAccept([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid authenticatedUserId = User.GetUserId();

        // You can't reject a friend request from yourself, that would be weird
        if (authenticatedUserId == userId)
            return BadRequest();

        bool success = await _userManager.AcceptFriendRequestAsync(authenticatedUserId, userId, cancellationToken);

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