using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthenticationController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The user account</returns>
    /// <response code="200">Returns users account</response>
    /// <response code="400">Error details</response>
    /// <response code="500">Error details</response>
    [RequestSizeLimit(1024)]
    [HttpPost("signin", Name = "AuthSignIn")]
    [Consumes(Application.Json, Application.Xml)]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SignIn([FromBody] Authentication.Models.AuthSignIn body, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return this.Error_AnonymousOnly();
        }

        await using TimeLock tl = TimeLock.FromSeconds(4, cancellationToken);

        DTOs.SignInResult signInResult = await _signInManager.PasswordSignInAsync(body.UserName, body.Password, cancellationToken);
        switch (signInResult.Result)
        {
            case DTOs.SignInResultType.Success:
                break;
            case DTOs.SignInResultType.UserNotFound:
            case DTOs.SignInResultType.PasswordInvalid:
                return this.Error_InvalidCredentials("Invalid username/password", "Please check that your entered username and password are correct", "username", "password");
            case DTOs.SignInResultType.LockedOut:
                // TODO: get lockout reason and end date
                return this.Error(StatusCodes.Status400BadRequest, "Account disabled", "Your account has been disabled. Please contact support for more information.");
            case DTOs.SignInResultType.EmailNotConfirmed:
                return this.Error(StatusCodes.Status400BadRequest, "Email not confirmed", "Please confirm your email address before signing in.");
            default:
            case DTOs.SignInResultType.InternalServerError:
                return this.Error_InternalServerError();
        }

        Data.Models.SignInEntity signIn = signInResult.SignIn!;
        Response.Cookies.Append("access_token", signIn.Id.ToString(), new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = signIn.ExpiresAt
        });

        return Ok(new Account.Models.AccountDto(signIn.User)); // TODO: use a mapper FFS
    }
}
