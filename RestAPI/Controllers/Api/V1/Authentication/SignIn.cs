using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.DTOs.API.Auth;
using ZapMe.Helpers;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="lockOutStore"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AnonymousOnly]
    [RequestSizeLimit(1024)]
    [HttpPost("signin", Name = "AuthSignIn")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
    public async Task<IActionResult> SignIn([FromBody] AuthSignIn body, [FromServices] ILockOutStore lockOutStore, [FromServices] IOptions<LegalOptions> options, CancellationToken cancellationToken)
    {
        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(2, cancellationToken);

        UserEntity? user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == body.UsernameOrEmail || u.Email == body.UsernameOrEmail, cancellationToken);
        if (user is null || !PasswordUtils.VerifyPassword(body.Password, user.PasswordHash))
        {
            return HttpErrors.Generic(
                    StatusCodes.Status401Unauthorized,
                    "invalid_credentials",
                    "Please check that your entered the correct username/email and password",
                    NotificationSeverityLevel.Warning,
                    "Invalid credentials"
                ).ToActionResult();
        }

        if (!user.EmailVerified)
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Unverified Email", "Email has not been verified", NotificationSeverityLevel.Warning, "Please verify your email address before signing in").ToActionResult();
        }

        LockOutEntity[] lockouts = await _dbContext.LockOuts
            .Where(u => u.UserId == user.Id)
            .ToArrayAsync(cancellationToken);
        if (lockouts.Any())
        {
            string reason = "Please contact support for more information";

            IEnumerable<LockOutEntity> publicList = lockouts.Where(static x => x.Flags.Split(',').Contains("public"));
            if (publicList.Any())
            {
                reason = "Reason(s):\n" + String.Join("\n", publicList.Select(static x => x.Reason));
            }

            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Account disabled", "Account has been disabled either by moderative or administrative reasons", NotificationSeverityLevel.Error, "Account disabled: " + reason).ToActionResult();
        }

        if (String.IsNullOrEmpty(user.Email))
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Unverified Email", "Email has not been verified", NotificationSeverityLevel.Error, "Please verify your email address before signing in").ToActionResult();
        }

        if (user.AcceptedPrivacyPolicyVersion < options.Value.PrivacyPolicyVersion)
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "review_privpol", "User needs to accept new Privacy Policy", NotificationSeverityLevel.Error, "Please read and accept the new Privacy Policy before creating an account").ToActionResult();
        }

        if (user.AcceptedTermsOfServiceVersion < options.Value.TermsOfServiceVersion)
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "review_tos", "User needs to accept new Terms of Service", NotificationSeverityLevel.Error, "Please read and accept the new Terms of Service before creating an account").ToActionResult();
        }

        string userAgent = this.GetRemoteUserAgent();

        if (userAgent.Length > UserAgentLimits.MaxUploadLength)
        {
            // Request body too large
            return HttpErrors.Generic(StatusCodes.Status413RequestEntityTooLarge, "User-Agent too long", $"User-Agent header has a hard limit on {UserAgentLimits.MaxUploadLength} characters", NotificationSeverityLevel.Error, "Unexpected behaviour, please contact developers").ToActionResult();
        }

        SessionEntity session = await _sessionManager.CreateAsync(user, this.GetRemoteIP(), this.GetCloudflareIPCountry(), userAgent, body.RememberMe, cancellationToken);

        AuthenticationProperties props = new AuthenticationProperties
        {
            IssuedUtc = session.CreatedAt,
            ExpiresUtc = session.ExpiresAt
        };

        return SignIn(new ClaimsPrincipal(session.ToClaimsIdentity()), props);
    }
}
