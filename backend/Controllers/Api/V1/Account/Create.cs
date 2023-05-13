using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using OneOf;
using ZapMe.Attributes;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Create a new account
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cfTurnstileService"></param>
    /// <param name="debounceService"></param>
    /// <param name="emailVerificationManager"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="201">Created account</response>
    /// <response code="400">Error details</response>
    /// <response code="409">Error details</response>
    [AnonymousOnly]
    [RequestSizeLimit(1024)]
    [HttpPost(Name = "CreateAccount")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(CreateOk), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status409Conflict)] // Username/email already taken
    public async Task<IActionResult> Create(
        [FromBody] CreateAccount body,
        [FromServices] ICloudflareTurnstileService cfTurnstileService,
        [FromServices] IDebounceService debounceService,
        [FromServices] IEmailVerificationManager emailVerificationManager,
        [FromServices] IOptions<ZapMeOptions> options,
        CancellationToken cancellationToken)
    {
        // Verify turnstile token
        CloudflareTurnstileVerifyResponse reCaptchaResponse = await cfTurnstileService.VerifyUserResponseTokenAsync(body.TurnstileResponse, this.GetRemoteIP(), cancellationToken);
        if (!reCaptchaResponse.Success)
        {
            if (reCaptchaResponse.ErrorCodes != null)
            {
                foreach (string errorCode in reCaptchaResponse.ErrorCodes)
                {
                    switch (errorCode)
                    {
                        case "missing-input-response": // The response parameter was not passed.
                            return CreateHttpError.InvalidModelState((nameof(body.TurnstileResponse), "Missing Cloudflare Turnstile Response")).ToActionResult();
                        case "invalid-input-response": // The response parameter is invalid or has expired.
                            return CreateHttpError.InvalidModelState((nameof(body.TurnstileResponse), "Invalid Cloudflare Turnstile Response")).ToActionResult();
                        case "timeout-or-duplicate": // The response parameter has already been validated before.
                            return CreateHttpError.InvalidModelState((nameof(body.TurnstileResponse), "Cloudflare Turnstile Response Expired or Already Used")).ToActionResult();
                        case "missing-input-secret": // The secret parameter was not passed.
                            _logger.LogError("Missing Cloudflare Turnstile Secret");
                            break;
                        case "invalid-input-secret": // The secret parameter was invalid or did not exist.
                            _logger.LogError("Invalid Cloudflare Turnstile Secret");
                            break;
                        case "bad-request": // The request was rejected because it was malformed.
                            _logger.LogError("Bad Cloudflare Turnstile Request");
                            break;
                        case "internal-error": // An internal error happened while validating the response. The request can be retried.
                            _logger.LogError("Cloudflare Turnstile Internal Error");
                            break;
                        default:
                            break;
                    };
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // Attempt to check against debounce if the email is a throwaway email
        if (await debounceService.IsDisposableEmailAsync(body.Email, cancellationToken))
        {
            return CreateHttpError.InvalidModelState((nameof(body.Email), "Disposable Emails are not allowed")).ToActionResult();
        }

        if (body.AcceptedPrivacyPolicyVersion < options.Value.PrivacyPolicyVersion)
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "review_privpol", "User needs to accept new Privacy Policy", UserNotification.SeverityLevel.Error, "Please read and accept the new Privacy Policy before creating an account").ToActionResult();
        }

        if (body.AcceptedTermsOfServiceVersion < options.Value.TermsOfServiceVersion)
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "review_tos", "User needs to accept new Terms of Service", UserNotification.SeverityLevel.Error, "Please read and accept the new Terms of Service before creating an account").ToActionResult();
        }

        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(2, cancellationToken);

        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Create account
        OneOf<UserEntity, ErrorDetails> tryCreateAccountResult = await _userManager.TryCreateAsync(body.Username, body.Email, body.Password, emailVerified: false, cancellationToken);
        if (tryCreateAccountResult.TryPickT1(out ErrorDetails errorDetails, out UserEntity user))
        {
            return errorDetails.ToActionResult();
        }

        // Send email verification
        ErrorDetails? test = await emailVerificationManager.InitiateEmailVerificationAsync(user, body.Email, cancellationToken);
        if (test.HasValue)
        {
            return test.Value.ToActionResult();
        }

        // Commit transaction
        await transaction.CommitAsync(cancellationToken);

        return CreatedAtAction(nameof(Get), new CreateOk { Id = user.Id, Message = "Account created, please check your email for a verification link" });
    }
}