using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        Guid thisUserId = User.GetUserId();

        var result = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Where(u => !u.RelationsOutgoing.Any(r => r.TargetUserId == thisUserId && r.RelationType == UserRelationType.Blocked))
            .Join(
                _dbContext.UserRelations,
                u => u.Id,
                r => r.TargetUserId,
                (u, r) => new { user = u, outgoingRelation = r }
            )
            .Where(x => x.outgoingRelation.SourceUserId == thisUserId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        return Ok(result.user.ToUserDto(result.outgoingRelation));
    }
}