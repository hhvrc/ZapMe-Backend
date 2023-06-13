using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Helpers;
using System.Security.Claims;

namespace ZapMe.Controllers.Api.V1;

partial class UserController
{
    /// <summary>
    /// Send friend request
    /// </summary>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpPost("i/{userId}/friendrequest", Name = "SendFriendRequest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestSend([FromRoute] Guid targetUserId, CancellationToken cancellationToken)
    {
        Guid? userId = User.GetUserId();
        if (!userId.HasValue) return HttpErrors.UnauthorizedActionResult;

        UserEntity? user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Include(u => u.Relations)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null) return HttpErrors.UnauthorizedActionResult;

        // Check if requesting user has blocked target user, or if user is trying to send a friend request to themselves
        if (user.Relations!.Any(u => u.TargetUserId == targetUserId && u.RelationType == UserRelationType.Blocked) || userId == targetUserId)
        {
            return BadRequest();
        }

        // Get target user if exists and has not blocked the requesting user
        UserEntity? targetUser = await _dbContext.Users
            .Where(u =>
                u.Id == targetUserId &&
                !u.Relations!.Any(r =>
                    r.TargetUserId == userId &&
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
        if (await _dbContext.FriendRequests.AnyAsync(r => r.SenderId == targetUserId && r.ReceiverId == userId, cancellationToken))
        {
            return NotFound();
        }

        await _dbContext.FriendRequests.AddAsync(new()
        {
            SenderId = userId.Value,
            ReceiverId = targetUserId
        }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(targetUserId);
    }
}