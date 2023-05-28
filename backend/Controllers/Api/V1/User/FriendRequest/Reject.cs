using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.User.Models;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Delete outgoing/Reject incoming friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpDelete("i/{userId}/friendrequest", Name = "DenyFriendRequest")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FriendRequestDeny([FromRoute] Guid userId)
    {
        return Ok(userId);
    }
}