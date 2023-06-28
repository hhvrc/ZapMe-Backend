using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs.API.User;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// List all incoming and outgoing friend requests
    /// </summary>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("friendrequests", Name = "GetFriendRequests")]
    [ProducesResponseType(typeof(FriendRequestList), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFriendRequests(CancellationToken cancellationToken)
    {
        Guid userId = User.GetUserId();

        UserEntity? user = await _userManager.GetByIdAsync(userId, userId, cancellationToken);

        return Ok(new FriendRequestList
        {
            Incoming = user?.RelationsIncoming?.Select(r => r.SourceUserId) ?? Enumerable.Empty<Guid>(),
            Outgoing = user?.RelationsOutgoing?.Select(r => r.TargetUserId) ?? Enumerable.Empty<Guid>()
        });
    }
}