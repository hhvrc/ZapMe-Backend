using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
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
    /// <param name="emailTemplateStore"></param>
    /// <param name="mailGunService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="201">Created account</response>
    /// <response code="400">Error details</response>
    /// <response code="409">Error details</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost(Name = "CreateAccount")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status409Conflict)] // Username/email already taken
    public async Task<IActionResult> Create(
        [FromBody] CreateAccount body,
        [FromServices] ICloudFlareTurnstileService cfTurnstileService,
        [FromServices] IDebounceService debounceService,
        [FromServices] IMailTemplateStore emailTemplateStore,
        [FromServices] IMailGunService mailGunService,
        CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return CreateHttpError.AnonymousOnly().ToActionResult();
        }

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

        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(4, cancellationToken);

        // Create account
        AccountCreationResult result = await _userManager.TryCreateAsync(body.UserName, body.Email, body.Password, cancellationToken);
        if (!result.IsSuccess)
        {
            switch (result.Result)
            {
                case AccountCreationResult.ResultE.Success:
                    break;
                case AccountCreationResult.ResultE.NameAlreadyTaken:
                case AccountCreationResult.ResultE.EmailAlreadyTaken:
                    return CreateHttpError.Generic(StatusCodes.Status409Conflict, "One or multiple idenitifiers in use", "Fields \"UserName\" or \"Email\" are not available", UserNotification.SeverityLevel.Warning, "Username/Email already taken", "Please choose a different Username or Email").ToActionResult();
                case AccountCreationResult.ResultE.NameOrEmailInvalid:
                    break;
                case AccountCreationResult.ResultE.UnknownError:
                    break;
                default:
                    break;
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        string? emailTemplate = await emailTemplateStore.GetTemplateAsync("AccountCreated", cancellationToken);
        if (emailTemplate != null)
        {
            string emailBody = new QuickStringReplacer(emailTemplate)
                    .Replace("{{UserName}}", body.UserName)
                    .Replace("{{ConfirmEmailLink}}", App.BackendUrl + "/Account/ConfirmEmail?token=")// + result.ConfirmationToken)
                    .Replace("{{ContactLink}}", App.ContactUrl)
                    .Replace("{{PrivacyPolicyLink}}", App.PrivacyPolicyUrl)
                    .Replace("{{TermsOfServiceLink}}", App.TermsOfServiceUrl)
                    .Replace("{{CompanyName}}", App.AppCreator)
                    .Replace("{{CompanyAddress}}", App.MadeInText)
                    .Replace("{{PoweredBy}}", App.AppName)
                    .Replace("{{PoweredByLink}}", App.WebsiteUrl)
                    .ToString();

            await mailGunService.SendEmailAsync("System", body.UserName, body.Email, "Account Created", emailTemplate, cancellationToken);
            throw new NotImplementedException();
        }
        else
        {
            _logger.LogError("Failed to load email template \"AccountCreated\"");
        }

        // TODO: Send email verification
        return CreatedAtAction(nameof(Get), new Account.Models.AccountDto(result.Entity)); // TODO: use a mapper FFS
    }
}