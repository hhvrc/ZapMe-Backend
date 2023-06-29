using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;

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
    [HttpPut("{userId}/friendrequest", Name = "AcceptFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]        // Accepted
    [ProducesResponseType(StatusCodes.Status304NotModified)] // Already friends
    [ProducesResponseType(StatusCodes.Status404NotFound)]    // No friendrequest incoming
    public async Task<IActionResult> FriendRequestAccept([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid fromUserId = User.GetUserId();

        // You can't reject a friend request from yourself, that would be weird
        if (fromUserId == userId)
            return BadRequest();

        var result = await _userManager.CreateOrAcceptFriendRequestAsync(fromUserId, userId, cancellationToken);

        return result switch
        {
            Enums.UpdateUserRelationResult.NoChanges => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.NotAllowed => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.AlreadyFriends => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.FriendshipCreated => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.CannotApplyToSelf => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
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