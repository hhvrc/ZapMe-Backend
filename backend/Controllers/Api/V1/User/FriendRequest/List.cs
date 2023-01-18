using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.User.FriendRequest.Models;
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
    [ProducesResponseType(typeof(FriendRequestList), StatusCodes.Status200OK)]
    public async Task<FriendRequestList> ListFriendRequests([FromServices] IFriendRequestStore friendRequestStore, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        FriendRequestEntity[] friendRequests = await friendRequestStore.ListByUserAsync(identity.AccountId).ToArrayAsync(cancellationToken); // TODO: improve performance

        return new FriendRequestList
        {
            Incoming = friendRequests.Where(fr => fr.ReceiverId == identity.AccountId).Select(fr => fr.SenderId),
            Outgoing = friendRequests.Where(fr => fr.SenderId == identity.AccountId).Select(fr => fr.ReceiverId)
        };
    }
}