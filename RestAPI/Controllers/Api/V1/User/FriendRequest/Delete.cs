using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Delete incoming/outgoing friend request
    /// </summary>
    [RequestSizeLimit(1024)]
    [HttpDelete("{userId}/friendrequest", Name = "DeleteFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status204NoContent)]
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
            UpdateUserRelationResult.Success => NoContent(), // TODO: Create a response DTO for this
            UpdateUserRelationResult.NoChanges => NotFound(), // TODO: Create a response DTO for this
            UpdateUserRelationResult.CannotApplyToSelf => BadRequest(), // TODO: Create a response DTO for this
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }
}