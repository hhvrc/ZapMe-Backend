using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Database.Models;
using ZapMe.DTOs.API.User;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using System.Security.Claims;

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
    public async Task<IActionResult> ListFriendRequests(CancellationToken cancellationToken)
    {
        Guid? userId = User.GetUserId();
        if (!userId.HasValue) return HttpErrors.UnauthorizedActionResult;

        FriendRequestEntity[] friendRequests = await _dbContext.FriendRequests
            .Where(fr => fr.ReceiverId == userId || fr.SenderId == userId)
            .ToArrayAsync(cancellationToken);

        return Ok(new FriendRequestList
        {
            Incoming = friendRequests.Where(fr => fr.ReceiverId == userId).Select(fr => fr.SenderId),
            Outgoing = friendRequests.Where(fr => fr.SenderId == userId).Select(fr => fr.ReceiverId)
        });
    }
}