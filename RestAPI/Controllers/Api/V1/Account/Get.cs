using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Account</response>
    [HttpGet(Name = "GetAccount")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        UserEntity? user = await User.GetUserAsync(_dbContext, cancellationToken);
        if (user == null)
        {
            return HttpErrors.UnauthorizedActionResult;
        }

        return Ok(user.ToAccountDto());
    }
}