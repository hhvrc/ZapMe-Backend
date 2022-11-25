using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// List all incoming and outgoing friend requests
    /// </summary>
    /// <returns></returns>
    /// <response code="200"></response>
    [RequestSizeLimit(1024)]
    [HttpGet("friendrequests", Name = "ListFriendRequests")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(User.FriendRequest.Models.FriendRequestList), StatusCodes.Status200OK)]
    public async Task<IActionResult> FriendRequestList([FromServices] IFriendRequestStore friendRequestStore, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        FriendRequestEntity[] friendRequests = await friendRequestStore.ListByUserAsync(identity.UserId, cancellationToken);

        return Ok(new User.FriendRequest.Models.FriendRequestList
        {
            Incoming = friendRequests.Where(fr => fr.ReceiverId == identity.UserId).Select(fr => fr.SenderId).ToArray(),
            Outgoing = friendRequests.Where(fr => fr.SenderId == identity.UserId).Select(fr => fr.ReceiverId).ToArray()
        });
    }
}