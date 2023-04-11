using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Services;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Complete the password reset request using the token that was received in the users email
    /// </summary>
    /// <param name="body"></param>
    /// <param name="passwordResetRequestStore"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    /// <response code="404">Error details</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("recover-confirm", Name = "AccountRecoveryConfirm")]
    [Consumes(Application.Json, Application.Xml)]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // Token invalid, expired, or already used
    public async Task<IActionResult> RecoveryConfirm([FromBody] Account.Models.RecoveryConfirm body, [FromServices] PasswordResetRequestStore passwordResetRequestStore, CancellationToken cancellationToken)
    {
        PasswordResetRequestEntity? passwordResetRequest = await passwordResetRequestStore.GetByTokenAsync(body.Token, cancellationToken);
        if (passwordResetRequest == null)
        {
            return this.Error(StatusCodes.Status400BadRequest, "Bad reset token", "The reset token is invalid, expired, or has already been used.", UserNotification.SeverityLevel.Error, "Bad reset token", "The reset token is invalid, expired, or has already been used.");
        }

        // Check if token is valid
        if (passwordResetRequest.CreatedAt.AddMinutes(30) < DateTime.UtcNow)
        {
            return this.Error(StatusCodes.Status400BadRequest, "Bad reset token", "The reset token is invalid, expired, or has already been used.", UserNotification.SeverityLevel.Error, "Bad reset token", "The reset token is invalid, expired, or has already been used.");
        }



        return Ok();
    }
}