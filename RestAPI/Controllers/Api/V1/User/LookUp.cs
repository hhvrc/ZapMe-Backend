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
        Guid thisUserId = User.GetUserId();

        var result = await _dbContext.Users
            .Where(u => u.Name == userName)
            .Select(u => new
            {
                user = u,
                outgoingRelation = u.RelationsOutgoing.FirstOrDefault(r => r.SourceUserId == thisUserId),
                incomingRelation = u.RelationsIncoming.FirstOrDefault(r => r.TargetUserId == thisUserId)
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.incomingRelation == null || u.incomingRelation.RelationType != UserRelationType.Blocked, cancellationToken);
        if (result is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        return Ok(result.user.ToUserDto(result.outgoingRelation));
    }
}