using Microsoft.AspNetCore.Authentication;
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
        string? sessionIdString;

        // First check if theres a authorization header, then check if theres a cookie
        if (Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader))
        {
            string[] authorizationHeaderParts = authorizationHeader.ToString().Split(' ');
            if (authorizationHeaderParts.Length != 2 || authorizationHeaderParts[0] != "Bearer")
            {
                return AuthenticateResult.Fail("Invalid authorization header.");
            }

            sessionIdString = authorizationHeaderParts[1];
        }
        else if (!Request.Cookies.TryGetValue(_options.CookieName, out sessionIdString))
        {
            return AuthenticateResult.NoResult();
        }

        if (String.IsNullOrEmpty(sessionIdString))
            return AuthenticateResult.Fail("Empty Login Cookie");

        if (!Guid.TryParse(sessionIdString, out Guid sessionId))
            return AuthenticateResult.Fail("Malformed Login Cookie");

        SessionEntity? session = await _sessionStore.GetByIdAsync(sessionId, _context.RequestAborted);
        if (session == null)
            return AuthenticateResult.Fail("Invalid Login Cookie");

        if (session.IsExpired)
            return AuthenticateResult.Fail("Expired Login Cookie");

        // TODO: some kinda refresh logic for the cookie if it's half way expired

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