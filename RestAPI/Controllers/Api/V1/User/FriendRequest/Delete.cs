using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;
using ZapMe.DTOs.API.User;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Delete incoming/outgoing friend request
    /// </summary>
    /// <response code="200">Deleted/Rejected request</response>
    /// <response code="404">No friend request found</response>
    [HttpDelete("{userId}/friendrequest", Name = "DeleteFriendRequest")]
    [ProducesResponseType(typeof(FriendRequestDelete200OkDto), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestDelete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userManager.DeleteOrRejectFriendRequestAsync(User.GetUserId(), userId, cancellationToken);

        return result switch
        {
            UpdateUserRelationResult.Success => NoContent(), // TODO: Create a response DTO for this
            UpdateUserRelationResult.NoChanges => NotFound(),
            UpdateUserRelationResult.CannotApplyToSelf => BadRequest(),
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }
}