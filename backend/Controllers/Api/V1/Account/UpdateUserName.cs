using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Helpers;
using ZapMe.Utils;

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
    [HttpPut("username", Name = "UpdateUserName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUserName body, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        if (identity.User.PasswordHash != PasswordUtils.HashPassword(body.Password))
        {
            return CreateHttpError.InvalidPassword().ToActionResult();
        }

        bool success = await _dbContext.Users
            .Where(u => u.Id == identity.UserId)
            .ExecuteUpdateAsync(spc => spc
                .SetProperty(u => u.Name, _ => body.NewUsername)
                .SetProperty(u => u.UpdatedAt, _ => DateTime.UtcNow)
                , cancellationToken) > 0;

        return success ? Ok() : CreateHttpError.InternalServerError().ToActionResult();
    }
}