using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Look up user by name
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("u/{userName}", Name = "LookUpUser")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]     // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> LookUp([FromRoute] string userName, CancellationToken cancellationToken)
    {
        Guid userId = User.GetUserId();

        string cacheKey = "zapme.users.byname:" + userName;

        UserEntity? user = await _cache.GetAsync<UserEntity>(cacheKey, cancellationToken);
        if (user is null)
        {
            user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == userName, cancellationToken: cancellationToken);
            if (user is null)
            {
                return HttpErrors.UserNotFoundActionResult;
            }

            await _cache.SetAsync(cacheKey, user, cancellationToken);
        }

        // If the target user has blocked the current user, return 404
        if (user.Relations.Any(r => r.TargetUserId == userId && r.RelationType == UserRelationType.Blocked))
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        // Else if the current user has blocked the target user, return minimal user info
        if (user.Relations.Any(r => r.SourceUserId == userId && r.RelationType == UserRelationType.Blocked))
        {
            return Ok(user.ToMinimalUserDto());
        }

        return Ok(user.ToUserDto());
    }
}