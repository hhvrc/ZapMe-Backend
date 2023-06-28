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
    /// Look up user by name
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("lookup", Name = "UserGetByName")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]     // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> LookUp([FromServices] DatabaseContext dbContext, [FromQuery] string username, CancellationToken cancellationToken)
    {
        var result = await UserFetchingLogic.FetchUserDto_AsUser_ByName(dbContext, User.GetUserId(), username, cancellationToken);

        if (!result.HasValue)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        return Ok(result.Value);
    }
}