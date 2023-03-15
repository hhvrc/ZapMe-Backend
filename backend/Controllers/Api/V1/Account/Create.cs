using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Constants;
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
    /// <param name="reCaptchaService"></param>
    /// <param name="debounceService"></param>
    /// <param name="emailTemplateStore"></param>
    /// <param name="mailGunService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Created account</response>
    /// <response code="400">Error details</response>
    /// <response code="409">Error details</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost(Name = "CreateAccount")]
    [Consumes(Application.Json, Application.Xml)]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status409Conflict)] // Username/email already taken
    public async Task<IActionResult> CreateAsync(
        [FromBody] Account.Models.Create body,
        [FromServices] IGoogleReCaptchaService reCaptchaService,
        [FromServices] IDebounceService debounceService,
        [FromServices] IMailTemplateStore emailTemplateStore,
        [FromServices] IMailGunService mailGunService,
        CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return this.Error_AnonymousOnly();
        }

        // Verify captcha
        GoogleReCaptchaVerifyResponse reCaptchaResponse = await reCaptchaService.VerifyUserResponseTokenAsync(body.ReCaptchaResponse, this.GetRemoteIP(), cancellationToken);
        if (!reCaptchaResponse.Success)
        {
            if (reCaptchaResponse.ErrorCodes != null)
            {
                foreach (string errorCode in reCaptchaResponse.ErrorCodes)
                {
                    switch (errorCode)
                    {
                        case "invalid-input-response":
                            return this.Error_InvalidModelState((nameof(body.ReCaptchaResponse), "Invalid ReCaptcha Response"));
                        case "timeout-or-duplicate":
                            return this.Error_InvalidModelState((nameof(body.ReCaptchaResponse), "ReCaptcha Response Expired or Already Used"));
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
            return this.Error_InvalidModelState((nameof(body.Email), "Disposable Emails are not allowed"));
        }

        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(4, cancellationToken);

        // Create account
        AccountCreationResult result = await _accountManager.TryCreateAsync(body.UserName, body.Email, body.Password, cancellationToken);
        if (!result.IsSuccess)
        {
            switch (result.Result)
            {
                case AccountCreationResult.ResultE.Success:
                    break;
                case AccountCreationResult.ResultE.NameAlreadyTaken:
                case AccountCreationResult.ResultE.EmailAlreadyTaken:
                    return this.Error(StatusCodes.Status409Conflict, "One or multiple idenitifiers in use", "Fields \"UserName\" or \"Email\" are not available", UserNotification.SeverityLevel.Warning, "Username/Email already taken", "Please choose a different Username or Email");
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
                    //.Replace("{{ConfirmEmailLink}}", App.BackendBaseUrl + "/Account/ConfirmEmail?token=" + result.ConfirmationToken)
                    .Replace("{{CompanyName}}", App.AppCreator)
                    .Replace("{{CompanyAddress}}", App.MadeInText)
                    .Replace("{{PoweredBy}}", App.AppName)
                    .Replace("{{PoweredByLink}}", App.WebsiteUrl)
                    .ToString();

            // TODO: change method signature to this: SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
            await mailGunService.SendEmailAsync("System", body.UserName, body.Email, "Account Created", emailTemplate, cancellationToken);
        }
        else
        {
            _logger.LogError("Failed to load email template \"AccountCreated\"");
        }

        // TODO: Send email verification
        return CreatedAtAction(nameof(Get), new Account.Models.AccountDto(result.Entity)); // TODO: use a mapper FFS
    }
}