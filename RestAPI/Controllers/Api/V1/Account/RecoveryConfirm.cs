using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Complete the password reset request using the token that was received in the users email
    /// </summary>
    /// <param name="body"></param>
    /// <param name="passwordResetManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("recover-confirm", Name = "AccountRecoveryConfirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Token invalid, expired, or already used
    public async Task<IActionResult> RecoveryConfirm([FromBody] RecoveryConfirm body, [FromServices] IPasswordResetManager passwordResetManager, CancellationToken cancellationToken)
    {
        if (!await passwordResetManager.TryCompletePasswordReset(body.Token, body.NewPassword, cancellationToken))
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Bad reset token", "The reset token is invalid, expired, or has already been used.", NotificationSeverityLevel.Error, "Token invalid, expired, or has already been used.").ToActionResult();
        }

        return Ok();
    }
}