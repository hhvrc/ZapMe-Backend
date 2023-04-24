﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Authentication.Models;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthenticationController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="lockOutStore"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The user account</returns>
    /// <response code="200">Returns SignInOk along with a Cookie with similar data</response>
    /// <response code="400">Error details</response>
    /// <response code="500">Error details</response>
    [RequestSizeLimit(1024)]
    [HttpPost("signin", Name = "AuthSignIn")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(SignInOk), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status413RequestEntityTooLarge)]
    public async Task<IActionResult> SignIn([FromBody] Authentication.Models.AuthSignIn body, [FromServices] ILockOutStore lockOutStore, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return CreateHttpError.AnonymousOnly().ToActionResult();
        }

        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(2, cancellationToken);

        UserEntity? user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == body.UsernameOrEmail || u.Email == body.UsernameOrEmail, cancellationToken);
        if (user == null || !PasswordUtils.CheckPassword(body.Password, user.PasswordHash))
        {
            return CreateHttpError.InvalidCredentials("Invalid username/email/password", "Please check that your entered username/email and password are correct", "username", "password").ToActionResult();
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

            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Account disabled", "Account has been disabled either by moderative or administrative reasons", UserNotification.SeverityLevel.Error, "Account disabled", reason).ToActionResult();
        }

        if (String.IsNullOrEmpty(user.Email))
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "Unverified Email", "Email has not been verified", UserNotification.SeverityLevel.Error, "Email not verified", "Please verify your email address before signing in").ToActionResult();
        }

        if (user.AcceptedTosVersion < 0) // TODO: have a currentTosVerion value
        {
            return CreateHttpError.Generic(StatusCodes.Status400BadRequest, "TOS Review Required", "User needs to accept new TOS", UserNotification.SeverityLevel.Error, "Terms of Service not accepted", "Please accept the Terms of Service before signing in").ToActionResult();
        }

        string userAgent = this.GetRemoteUserAgent();

        if (userAgent.Length > UserAgentLimits.MaxUploadLength)
        {
            // Request body too large
            return CreateHttpError.Generic(StatusCodes.Status413RequestEntityTooLarge, "User-Agent too long", $"User-Agent header has a hard limit on {UserAgentLimits.MaxUploadLength} characters", UserNotification.SeverityLevel.Error, "Bad client behaviour", "Your client has unexpected behaviour, please contact it's developers").ToActionResult();
        }

        SessionEntity session = await _sessionManager.CreateAsync(user, body.SessionName, this.GetRemoteIP(), this.GetCloudflareIPCountry(), userAgent, body.RememberMe, cancellationToken);

        session.User = user;

        return SignIn(new ZapMePrincipal(session));
    }
}
