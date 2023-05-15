using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Controllers.Api.V1.User.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="404"></response>
    [RequestSizeLimit(1024)]
    [HttpGet("i/{userId}", Name = "GetUser")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(User.Models.UserDto), StatusCodes.Status200OK)]     // Accepted
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        UserEntity user = (User as ZapMePrincipal)!.Identity.User;

        UserEntity? targetUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId && !u.Relations!.Any(r => r.TargetUserId == user.Id || r.SourceUserId == user.Id), cancellationToken);
        if (targetUser == null)
        {
            return CreateHttpError.Generic(StatusCodes.Status404NotFound, "Not found", $"User with id {userId} not found").ToActionResult();
        }

        // TODO: use a mapper
        return Ok(new UserDto(targetUser));
    }
}