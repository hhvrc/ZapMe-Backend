using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Account.Models;
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
    /// <param name="passwordResetManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("recover", Name = "AccountRecoveryRequest")]
    [Consumes(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoveryRequest([FromBody] RecoveryRequest body, [FromServices] ICloudflareTurnstileService cfTurnstileService, [FromServices] IPasswordResetManager passwordResetManager, CancellationToken cancellationToken)
    {
        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(2, cancellationToken);

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
                            return CreateHttpError.InvalidModelState((nameof(body.TurnstileResponse), "Invalid ReCaptcha Response")).ToActionResult();
                        case "timeout-or-duplicate":
                            return CreateHttpError.InvalidModelState((nameof(body.TurnstileResponse), "ReCaptcha Response Expired or Already Used")).ToActionResult();
                        default:
                            break;
                    };
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        await passwordResetManager.InitiatePasswordReset(body.Email, cancellationToken);

        // Return 200 OK no matter what happens, this is to prevent email enumeration
        return Ok(new RecoveryRequestOk { Message = "If the email you provided is registered, a recovery email has been sent to it. Please check your inbox." });
    }
}