using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account email
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">New email</response>
    /// <response code="400">Error details</response>
    [RequestSizeLimit(1024)]
    [HttpPut("email", Name = "UpdateEmail")]
    [Consumes(Application.Json, Application.Xml)]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEmail([FromBody] Account.Models.UpdateEmail body, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        PasswordCheckResult result = await _accountManager.CheckPasswordAsync(identity.AccountId, body.Password, cancellationToken);
        if (result != PasswordCheckResult.Success)
        {
            return this.Error_InvalidPassword();
        }

        await _accountManager.Store.SetEmailAsync(identity.AccountId, body.NewEmail, cancellationToken);

        return Ok();
    }
}