using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Request password recovery of a account, a recovery email will be sent to the user that makes a call to the /recovery-confirm endpoint
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cfTurnstileService"></param>
    /// <param name="passwordResetRequestManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Account</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("recover", Name = "AccountRecoveryRequest")]
    [Consumes(Application.Json, Application.Xml)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoveryRequest([FromBody] Account.Models.RecoveryRequest body, [FromServices] ICloudFlareTurnstileService cfTurnstileService, [FromServices] IPasswordResetRequestManager passwordResetRequestManager, CancellationToken cancellationToken)
    {
        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(1, cancellationToken);

        CloudflareTurnstileVerifyResponse reCaptchaResponse = await cfTurnstileService.VerifyUserResponseTokenAsync(body.TurnstileResponse, this.GetRemoteIP(), cancellationToken);
        if (!reCaptchaResponse.Success)
        {
            if (reCaptchaResponse.ErrorCodes != null)
            {
                foreach (string errorCode in reCaptchaResponse.ErrorCodes)
                {
                    switch (errorCode)
                    {
                        case "invalid-input-response":
                            return this.Error_InvalidModelState((nameof(body.TurnstileResponse), "Invalid ReCaptcha Response"));
                        case "timeout-or-duplicate":
                            return this.Error_InvalidModelState((nameof(body.TurnstileResponse), "ReCaptcha Response Expired or Already Used"));
                        default:
                            break;
                    };
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        bool success = await passwordResetRequestManager.InitiatePasswordReset(body.Email, cancellationToken);

        return Ok();
    }
}