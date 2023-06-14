using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

partial class UserController
{
    /// <summary>
    /// Send friend request
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpPost("i/{userId}/friendrequest", Name = "SendFriendRequest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestSend([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid? authenticatedUserId = User.GetUserId();
        if (!authenticatedUserId.HasValue) return HttpErrors.UnauthorizedActionResult;

        UserEntity? user = await _dbContext.Users
            .Where(u => u.Id == authenticatedUserId)
            .Include(u => u.Relations)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null) return HttpErrors.UnauthorizedActionResult;

        // Check if requesting user has blocked target user, or if user is trying to send a friend request to themselves
        if (user.Relations!.Any(u => u.TargetUserId == userId && u.RelationType == UserRelationType.Blocked) || authenticatedUserId == userId)
        {
            return BadRequest();
        }

        // Get target user if exists and has not blocked the requesting user
        UserEntity? targetUser = await _dbContext.Users
            .Where(u =>
                u.Id == userId &&
                !u.Relations!.Any(r =>
                    r.TargetUserId == authenticatedUserId &&
                    r.RelationType == UserRelationType.Blocked
                )
            )
            .Include(u => u.Relations)
            .FirstOrDefaultAsync(cancellationToken);
        if (targetUser is null)
        {
            return NotFound();
        }

        // Check if target user has already sent a friend request
        if (await _dbContext.FriendRequests.AnyAsync(r => r.SenderId == userId && r.ReceiverId == authenticatedUserId, cancellationToken))
        {
            return NotFound();
        }

        await _dbContext.FriendRequests.AddAsync(new()
        {
            SenderId = authenticatedUserId.Value,
            ReceiverId = userId
        }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(userId);
    }
}