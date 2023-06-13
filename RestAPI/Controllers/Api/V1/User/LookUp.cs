using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using System.Security.Claims;

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
        Guid? userId = User.GetUserId();
        if (!userId.HasValue) return HttpErrors.UnauthorizedActionResult;

        UserEntity? targetUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == userName && !u.Relations!.Any(r => r.TargetUserId == userId || r.SourceUserId == userId), cancellationToken);
        if (targetUser == null)
        {
            return HttpErrors.Generic(StatusCodes.Status404NotFound, "Not found", $"User with nane {userName} not found").ToActionResult();
        }

        return Ok(targetUser.ToUserDto());
    }
}