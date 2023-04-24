using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Accept incoming friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="304"></response>
    /// <response code="404"></response>
    [RequestSizeLimit(1024)]
    [HttpPut("i/{userId}/friendrequest", Name = "AcceptFriendRequest")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(User.Models.UserDto), StatusCodes.Status200OK)]        // Accepted
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status304NotModified)] // Already friends
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]    // No friendrequest incoming
    public IActionResult FriendRequestAccept([FromRoute] Guid userId)
    {
        return Ok(userId);
    }
}