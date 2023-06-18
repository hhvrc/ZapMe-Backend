using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ZapMe.Attributes;
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

        UserEntity? user = await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Name == body.UsernameOrEmail || u.Email == body.UsernameOrEmail, cancellationToken);
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
            return HttpErrors.UnverifiedEmailActionResult;
        }

        LockOutEntity[] lockouts = await _dbContext
            .LockOuts
            .Where(u => u.UserId == user.Id)
            .AsNoTracking()
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

        if (user.AcceptedPrivacyPolicyVersion < options.Value.PrivacyPolicyVersion)
        {
            return HttpErrors.ReviewPrivacyPolicyActionResult;
        }

        if (user.AcceptedTermsOfServiceVersion < options.Value.TermsOfServiceVersion)
        {
            return HttpErrors.ReviewTermsOfServiceActionResult;
        }

        string userAgent = this.GetRemoteUserAgent();

        if (userAgent.Length > UserAgentLimits.MaxUploadLength)
        {
            return HttpErrors.UserAgentTooLongActionResult;
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
