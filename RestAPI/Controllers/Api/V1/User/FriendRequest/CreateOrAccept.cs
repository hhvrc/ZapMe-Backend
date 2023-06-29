using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Create a new friend request or accept an incoming friend request to this user
    /// </summary>
    [RequestSizeLimit(1024)]
    [HttpPut("{userId}/friendrequest", Name = "CreateOrAcceptFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]        // Accepted
    [ProducesResponseType(StatusCodes.Status304NotModified)] // Already friends
    [ProducesResponseType(StatusCodes.Status404NotFound)]    // No friendrequest incoming
    public async Task<IActionResult> FriendRequestCreateOrAccept([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid fromUserId = User.GetUserId();

        // You can't reject a friend request from yourself, that would be weird
        if (fromUserId == userId)
            return BadRequest();

        var result = await _userManager.CreateOrAcceptFriendRequestAsync(fromUserId, userId, cancellationToken);

        return result switch
        {
            CreateOrAcceptFriendRequestResult.NoChanges => NoContent(), // TODO: Create a response DTO for this
            CreateOrAcceptFriendRequestResult.NotAllowed => BadRequest(), // TODO: Create a response DTO for this
            CreateOrAcceptFriendRequestResult.AlreadyFriends => NoContent(), // TODO: Create a response DTO for this
            CreateOrAcceptFriendRequestResult.FriendshipCreated => NoContent(), // TODO: Create a response DTO for this
            CreateOrAcceptFriendRequestResult.CannotApplyToSelf => BadRequest(), // TODO: Create a response DTO for this
            _ => HttpErrors.InternalServerErrorActionResult,
        };
        /*
            ? Ok()
            : HttpErrors.Generic(
                StatusCodes.Status404NotFound,
                "friendrequest_not_found",
                "Friend request not found",
                NotificationSeverityLevel.Warning,
                "Friend request not found"
              ).ToActionResult();
        */
    }
}