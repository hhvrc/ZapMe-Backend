using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account password
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    [RequestSizeLimit(1024)]
    [HttpPut("password", Name = "UpdatePassword")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdatePassword body, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        string oldPasswordHash = PasswordUtils.HashPassword(body.CurrentPassword);
        string newPasswordHash = PasswordUtils.HashPassword(body.NewPassword);

        // Do password update as a single atomic operation
        bool success = await _dbContext.Users
            .Where(u => u.Id == identity.UserId && u.PasswordHash == oldPasswordHash)
            .ExecuteUpdateAsync(spc => spc
                .SetProperty(u => u.PasswordHash, _ => newPasswordHash)
                .SetProperty(u => u.UpdatedAt, _ => DateTime.UtcNow)
                , cancellationToken) > 0;

        return success ? Ok() : CreateHttpError.InvalidPassword(UpdatePassword.CurrentPassword_JsonName).ToActionResult();
    }
}