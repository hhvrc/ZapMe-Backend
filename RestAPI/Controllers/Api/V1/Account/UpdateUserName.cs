using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs.API.User;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account username
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    [RequestSizeLimit(1024)]
    [HttpPut("username", Name = "AccountUsernameUpdate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUserName body, CancellationToken cancellationToken)
    {
        UserEntity? user = await User.VerifyUserPasswordAsync(body.Password, _dbContext, cancellationToken);
        if (user is null)
        {
            return HttpErrors.InvalidPasswordActionResult;
        }

        bool success = await _dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(spc => spc
                .SetProperty(u => u.Name, _ => body.NewUsername)
                .SetProperty(u => u.UpdatedAt, _ => DateTime.UtcNow)
                , cancellationToken) > 0;

        return success ? Ok() : HttpErrors.InternalServerErrorActionResult;
    }
}