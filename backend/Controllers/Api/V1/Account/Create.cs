using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
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
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status409Conflict)] // Username/email already taken
    public async Task<IActionResult> CreateAsync([FromBody] Account.Models.Create body, [FromServices] IGoogleReCaptchaService reCaptchaService, [FromServices] IDebounceService debounceService, CancellationToken cancellationToken)
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

        await using TimeLock tl = TimeLock.FromSeconds(4, cancellationToken);

        string username = body.UserName.TrimAndMinifyWhiteSpaces();

        // Create account
        UserEntity? user = await _userManager.TryCreateAsync(username, body.Email, body.Password, cancellationToken);
        if (user == null)
        {
            return this.Error_InvalidModelState((nameof(body.UserName), "Username/Email already taken"), (nameof(body.Email), "Username/Email already taken"));
        }

        return Ok(new Account.Models.AccountDto(user)); // TODO: use a mapper FFS
    }
}