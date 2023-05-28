using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.User.Models;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Accept incoming friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpPut("i/{userId}/friendrequest", Name = "AcceptFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]        // Accepted
    [ProducesResponseType(StatusCodes.Status304NotModified)] // Already friends
    [ProducesResponseType(StatusCodes.Status404NotFound)]    // No friendrequest incoming
    public IActionResult FriendRequestAccept([FromRoute] Guid userId)
    {
        return Ok(userId);
    }
}