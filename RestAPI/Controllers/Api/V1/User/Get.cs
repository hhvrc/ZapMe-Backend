using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using System.Security.Claims;
using ZapMe.Enums;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("i/{userId}", Name = "GetUser")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)] // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)]            // User not found
    public async Task<IActionResult> Get([FromRoute] Guid targetUserId, CancellationToken cancellationToken)
    {
        Guid? userId = User.GetUserId();
        if (!userId.HasValue) return HttpErrors.UnauthorizedActionResult;

        // VVVV TODO: move this to a more logical place VVVV
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
            return HttpErrors.UserNotFoundActionResult;
        }

        // Avoid users to see details of blocked users
        if (targetUser.Relations!.Any(r => r.SourceUserId == userId && r.RelationType == UserRelationType.Blocked))
        {
            return Ok(targetUser.ToMinimalUserDto());
        }

        return Ok(targetUser.ToUserDto());
    }
}