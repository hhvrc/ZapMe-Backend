using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthenticationController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="userManager"></param>
    /// <param name="passwordHasher"></param>
    /// <param name="lockOutManager"></param>
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
    public async Task<IActionResult> SignIn([FromBody] Authentication.Models.AuthSignIn body, [FromServices] IAccountManager userManager, [FromServices] IPasswordHasher passwordHasher, [FromServices] ILockOutManager lockOutManager, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return this.Error_AnonymousOnly();
        }

        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(4, cancellationToken);

        AccountEntity? account = await userManager.GetByNameAsync(body.Username, cancellationToken) ?? await userManager.GetByEmailAsync(body.Username, cancellationToken);
        if (account == null)
        {
            return this.Error_InvalidCredentials("Invalid username/password", "Please check that your entered username and password are correct", "username", "password");
        }

        if (!passwordHasher.CheckPassword(body.Password, account.PasswordHash))
        {
            return this.Error_InvalidCredentials("Invalid username/password", "Please check that your entered username and password are correct", "username", "password");
        }

        LockOutEntity[] lockouts = await lockOutManager.ListByUserIdAsync(account.Id, cancellationToken);
        if (lockouts.Any())
        {
            string reason = "Please contact support for more information";

            IEnumerable<LockOutEntity> publicList = lockouts.Where(static x => x.Flags.Split(',').Contains("public"));
            if (publicList.Any())
            {
                reason = "Reason(s):\n" + String.Join("\n", publicList.Select(static x => x.Reason));
            }

            return this.Error(StatusCodes.Status400BadRequest, "Account disabled", "Account has been disabled either by moderative or administrative reasons", UserNotification.SeverityLevel.Error, "Account disabled", reason);
        }

        if (!account.EmailVerified)
        {
            return this.Error(StatusCodes.Status400BadRequest, "Email not verified", "Please verify your email address before signing in", UserNotification.SeverityLevel.Error, "Email not verified", "Please verify your email address before signing in");
        }

        if (account.AcceptedTosVersion < 0) // TODO: have a currentTosVerion value
        {
            return this.Error(StatusCodes.Status400BadRequest, "Terms of Service not accepted", "Please accept the Terms of Service before signing in", UserNotification.SeverityLevel.Error, "Terms of Service not accepted", "Please accept the Terms of Service before signing in");
        }

        SessionEntity session = await _sessionManager.CreateAsync(account.Id, body.SessionName, this.GetRemoteIP(), this.GetRemoteUserAgent(), this.GetRemoteUserAgent(), body.RememberMe, cancellationToken);

        return SignIn(new ZapMePrincipal(session), ZapMeAuthenticationDefaults.AuthenticationScheme);
    }
}
