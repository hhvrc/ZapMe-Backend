﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using ZapMe.Authentication.Models;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationHandler : IAuthenticationSignInHandler
{
    private AuthenticationScheme _scheme = default!;
    private HttpContext _context = default!;
    private ZapMeAuthenticationOptions _options = default!;
    private Task<AuthenticateResult>? _authenticateTask = null;
    private readonly ISessionStore _sessionStore;
    private readonly IOptionsMonitor<ZapMeAuthenticationOptions> _optionsMonitor;
    private readonly ILogger<ZapMeAuthenticationHandler> _logger;

    public ZapMeAuthenticationHandler(ISessionStore sessionStore, IOptionsMonitor<ZapMeAuthenticationOptions> optionsMonitor, ILogger<ZapMeAuthenticationHandler> logger)
    {
        _sessionStore = sessionStore;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    private HttpRequest Request => _context.Request;
    private HttpResponse Response => _context.Response;

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _options = _optionsMonitor.Get(_scheme.Name);

        return Task.CompletedTask;
    }

    public async Task SignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        SessionEntity session = (claimsIdentity as ZapMePrincipal)!.Identity.Session;

        Response.StatusCode = StatusCodes.Status200OK;
        Response.Cookies.Append(
            _options.CookieName,
            session.Id.ToString(),
            new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                MaxAge = session.ExpiresAt - DateTime.UtcNow,
                IsEssential = true,
                Secure = true
            }
        );

        await Response.WriteAsJsonAsync(new SignInOk { SessionId = session.Id, IssuedAtUtc = session.CreatedAt, ExpiresAtUtc = session.ExpiresAt });
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        Response.Cookies.Delete(_options.CookieName);

        return Task.CompletedTask;
    }
    
    private async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? sessionIdString = Request.Headers["Authorization"];
        if (sessionIdString is not null)
        {
            if (!sessionIdString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Invalid Authorization header format.");
            }

            sessionIdString = sessionIdString["Bearer ".Length..].Trim();
        }
        else
        {
            sessionIdString = Request.Cookies[_options.CookieName];
        }
        
        if (sessionIdString == null)
        {
            return AuthenticateResult.NoResult();
        }

        if (!Guid.TryParse(sessionIdString, out Guid sessionId))
            return AuthenticateResult.Fail("Malformed Login Cookie");

        CancellationToken cancellationToken = _context.RequestAborted;

        SessionEntity? session = await _sessionStore.GetByIdAsync(sessionId, cancellationToken);
        if (session == null)
            return AuthenticateResult.Fail("Invalid Login Cookie");

        if (session.IsExpired)
            return AuthenticateResult.Fail("Expired Login Cookie");

        // TODO: is this good or bad?
        // Sliding window refresh
        if (session.IsHalfwayExpired)
        {
            // refresh session
            _ = _sessionStore.SetExipresAtAsync(session.Id, DateTime.UtcNow.Add(session.TimeToLive), cancellationToken);
        }

        ZapMePrincipal principal = new ZapMePrincipal(session);

        _context.User = principal; // TODO: is this needed?

        return AuthenticateResult.Success(new AuthenticationTicket(principal, ZapMeAuthenticationDefaults.AuthenticationScheme));
    }

    public Task<AuthenticateResult> AuthenticateAsync()
    {
        // Calling Authenticate more than once should always return the original value.
        return _authenticateTask ??= HandleAuthenticateAsync();
    }

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}