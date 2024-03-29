﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.DTOs;
using ZapMe.DTOs.API.User;
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
    [HttpPost("password-reset/request", Name = "RequestAccountPasswordReset")]
    [Consumes(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoveryRequest([FromBody] RecoveryRequest body, [FromServices] ICloudflareTurnstileService cfTurnstileService, [FromServices] IPasswordResetManager passwordResetManager, CancellationToken cancellationToken)
    {
        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(2, cancellationToken);

        CloudflareTurnstileVerifyResponseDto reCaptchaResponse = await cfTurnstileService.VerifyUserResponseTokenAsync(body.TurnstileResponse, this.GetRemoteIP(), cancellationToken);
        if (!reCaptchaResponse.Success)
        {
            if (reCaptchaResponse.ErrorCodes is not null)
            {
                foreach (string errorCode in reCaptchaResponse.ErrorCodes)
                {
                    switch (errorCode)
                    {
                        case "invalid-input-response":
                            return HttpErrors.InvalidModelState((nameof(body.TurnstileResponse), "Invalid ReCaptcha Response")).ToActionResult();
                        case "timeout-or-duplicate":
                            return HttpErrors.InvalidModelState((nameof(body.TurnstileResponse), "ReCaptcha Response Expired or Already Used")).ToActionResult();
                        default:
                            break;
                    };
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var user = await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == body.Email, cancellationToken);
        if (user is not null)
        {
            await passwordResetManager.InitiatePasswordReset(user, cancellationToken);
        }

        // Return 200 OK no matter what happens, this is to prevent email enumeration
        return Ok(new RecoveryRequestOk { Message = "If the email you provided is registered, a recovery email has been sent to it. Please check your inbox." });
    }
}