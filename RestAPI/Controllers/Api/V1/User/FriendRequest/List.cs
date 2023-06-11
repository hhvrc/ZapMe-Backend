using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.User.FriendRequest.Models;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// List all incoming and outgoing friend requests
    /// </summary>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("friendrequests", Name = "ListFriendRequests")]
    [ProducesResponseType(typeof(FriendRequestList), StatusCodes.Status200OK)]
    public async Task<FriendRequestList> ListFriendRequests([FromServices] IFriendRequestStore friendRequestStore, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        FriendRequestEntity[] friendRequests = await _dbContext.FriendRequests
            .Where(fr => fr.ReceiverId == identity.UserId || fr.SenderId == identity.UserId)
            .ToArrayAsync(cancellationToken);

        return new FriendRequestList
        {
            Incoming = friendRequests.Where(fr => fr.ReceiverId == identity.UserId).Select(fr => fr.SenderId),
            Outgoing = friendRequests.Where(fr => fr.SenderId == identity.UserId).Select(fr => fr.ReceiverId)
        };
    }
}