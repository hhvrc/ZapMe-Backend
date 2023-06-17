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
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FriendRequestSend([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid? authenticatedUserId = User.GetUserId();
        if (!authenticatedUserId.HasValue)
            return HttpErrors.UnauthorizedActionResult;

        // You cannot send a friend request to yourself
        if (authenticatedUserId == userId)
            return BadRequest();

        UserEntity[] users = await _dbContext.Users
            .Where(u => u.Id == authenticatedUserId || u.Id == userId)
            .ToArrayAsync(cancellationToken);
        if (users.Length < 2)
            return HttpErrors.UserNotFoundActionResult;
        if (users.Length > 2)
        {
            _logger.LogError("Found more than 2 users with the same ID. This should never happen.");
            return HttpErrors.InternalServerErrorActionResult;
        }

        // Check if authenticated user exists
        UserEntity? authenticatedUser = users.SingleOrDefault(u => u.Id == authenticatedUserId);
        if (authenticatedUser is null)
            return HttpErrors.UnauthorizedActionResult;

        // Check if authenticated user has blocked the target user
        if (authenticatedUser.RelationsOutgoing.Any(r => r.TargetUserId == userId && r.RelationType == UserRelationType.Blocked))
            return BadRequest();

        // Check if target user exists and has not blocked the requesting user
        UserEntity? targetUser = users.SingleOrDefault(u => u.Id == userId);
        if (targetUser is null || targetUser.RelationsOutgoing.Any(r => r.TargetUserId == authenticatedUserId && r.RelationType == UserRelationType.Blocked))
            return HttpErrors.UserNotFoundActionResult;

        // Check if friend request has already been sent
        if (authenticatedUser.FriendRequestsOutgoing!.Any(r => r.ReceiverId == userId))
            return StatusCode(StatusCodes.Status304NotModified);

        // Check if target user has already sent a friend request
        if (targetUser.FriendRequestsOutgoing!.Any(r => r.ReceiverId == authenticatedUserId))
        {
            // TODO URGENT: accept friend request and raise notification
            throw new NotImplementedException();
        }

        await _dbContext.FriendRequests.AddAsync(new()
        {
            SenderId = authenticatedUserId.Value,
            ReceiverId = userId
        }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // TODO: raise notification

        return Ok(userId);
    }
}