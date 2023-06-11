using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Database.Models;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.User.Models;
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
        UserEntity user = (User as ZapMePrincipal)!.Identity.User;

        UserEntity? targetUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == userName && !u.Relations!.Any(r => r.TargetUserId == user.Id || r.SourceUserId == user.Id), cancellationToken);
        if (targetUser == null)
        {
            return HttpErrors.Generic(StatusCodes.Status404NotFound, "Not found", $"User with nane {userName} not found").ToActionResult();
        }

        // TODO: use a mapper
        return Ok(new UserDto(targetUser));
    }
}