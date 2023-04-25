using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

partial class UserController
{
    /// <summary>
    /// Send friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="404"></response>
    [RequestSizeLimit(1024)]
    [HttpPost("i/{userId}/friendrequest", Name = "SendFriendRequest")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FriendRequestSend([FromRoute] Guid userId)
    {
        return Ok(userId);
    }
}