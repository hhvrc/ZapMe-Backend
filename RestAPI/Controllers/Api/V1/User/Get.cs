using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users;
using ZapMe.Database;
using ZapMe.DTOs;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Get user by Id
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("{userId}", Name = "UserGetById")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)] // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)]            // User not found
    public async Task<IActionResult> Get([FromServices] DatabaseContext dbContext, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await UserFetchingLogic.FetchUserDto_AsUser_ById(dbContext, User.GetUserId(), userId, cancellationToken);

        if (!result.HasValue)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        return Ok(result.Value);
    }
}