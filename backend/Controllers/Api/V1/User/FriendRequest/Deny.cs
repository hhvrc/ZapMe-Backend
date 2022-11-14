using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Delete outgoing/Reject incoming friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="404"></response>
    [RequestSizeLimit(1024)]
    [HttpDelete("i/{userId}/friendrequest", Name = "DenyFriendRequest")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(User.Models.User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
    public IActionResult FriendRequestDeny([FromRoute] Guid userId)
    {
        return Ok(userId);
    }
}