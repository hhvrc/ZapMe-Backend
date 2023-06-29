using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessRules;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Look up user by name
    /// </summary>
    /// <response code="302">User found</response>
    /// <response code="404">User not found</response>
    [HttpGet("lookup", Name = "GetUserByName")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LookUp([FromQuery] string username, [FromServices] IUserRepository userRepository, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUserNameAsync(username, cancellationToken);

        if (user is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        return RedirectToAction(nameof(Get), new { userId = user.Id });
    }
}