using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;

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
    [HttpDelete("{userId}/friendrequest", Name = "DeleteFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestDelete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid authenticatedUserId = User.GetUserId();

        // You can't reject a friend request from yourself, that would be weird
        if (authenticatedUserId == userId)
            return BadRequest();

        var result = await _userManager.DeleteOrRejectFriendRequestAsync(authenticatedUserId, userId, cancellationToken);

        return result switch
        {
            Enums.UpdateUserRelationResult.Success => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.NoChanges => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.NotAllowed => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.AlreadyFriends => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.FriendshipCreated => throw new NotImplementedException(),
            Enums.UpdateUserRelationResult.CannotApplyToSelf => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
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