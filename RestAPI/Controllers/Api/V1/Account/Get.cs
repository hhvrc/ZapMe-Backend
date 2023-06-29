using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    [HttpGet(Name = "GetAccount")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromServices] IUserRepository userRepo, CancellationToken cancellationToken)
    {
        var user = await userRepo.GetUserByIdAsync(User.GetUserId(), cancellationToken);

        if (user is null)
        {
            return HttpErrors.UnauthorizedActionResult;
        }

        return Ok(UserMapper.MapToAccountDto(user));
    }
}