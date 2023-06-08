using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account email
    /// </summary>
    /// <param name="body"></param>
    /// <param name="emailVerificationManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    [RequestSizeLimit(1024)]
    [HttpPut("email", Name = "UpdateEmail")]
    [ProducesResponseType(typeof(UpdateEmailOk), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmail body, [FromServices] IEmailVerificationManager emailVerificationManager, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        if (!PasswordUtils.CheckPassword(body.Password, identity.User.PasswordHash))
        {
            return HttpErrors.InvalidPassword().ToActionResult();
        }

        ErrorDetails? errorDetails = await emailVerificationManager.InitiateEmailVerificationAsync(identity.User, body.NewEmail, cancellationToken);
        if (errorDetails.HasValue)
        {
            return errorDetails.Value.ToActionResult();
        }

        return Ok(new UpdateEmailOk { Message = "Please check your email to verify your new address." });
    }
}