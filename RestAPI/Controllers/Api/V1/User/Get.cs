using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("i/{userId}", Name = "GetUser")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)] // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)]            // User not found
    public async Task<IActionResult> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid? authenticatedUserId = User.GetUserId();
        if (!authenticatedUserId.HasValue) return HttpErrors.UnauthorizedActionResult;

        // VVVV TODO: move this to a more logical place VVVV
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
            return HttpErrors.UserNotFoundActionResult;
        }

        // Avoid users to see details of blocked users
        if (targetUser.Relations!.Any(r => r.SourceUserId == authenticatedUserId && r.RelationType == UserRelationType.Blocked))
        {
            return Ok(targetUser.ToMinimalUserDto());
        }

        return Ok(targetUser.ToUserDto());
    }
}