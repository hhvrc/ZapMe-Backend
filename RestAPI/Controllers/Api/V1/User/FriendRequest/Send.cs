using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ZapMe.Controllers.Api.V1;

partial class UserController
{
    /// <summary>
    /// Send friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpPost("{userId}/friendrequest", Name = "SendFriendRequest")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestSend([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid fromUserId = User.GetUserId();

        if (fromUserId == userId)
            return BadRequest();

        var result = await _userManager.CreateOrAcceptFriendRequestAsync(fromUserId, userId, cancellationToken);
        /*
        if (!success)
        {
            // TODO: better error handling
            return HttpErrors.Generic(
                StatusCodes.Status404NotFound,
                "friendrequest_not_found",
                "Friend request not found",
                NotificationSeverityLevel.Warning,
                "Friend request not found"
               ).ToActionResult();
        }

        // TODO: raise notification
        */
        return NoContent();
    }
}