using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
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
    private string CookieName => _options.Cookie.Name ?? throw new InvalidOperationException("Cookie name cannot be null");

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _options = _optionsMonitor.Get(_scheme.Name);

        return Task.CompletedTask;
    }

    public async Task SignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        ArgumentNullException.ThrowIfNull(claimsIdentity, "ClaimsPrincipal cannot be null.");

        if (claimsIdentity is not ZapMePrincipal user)
        {
            throw new InvalidOperationException("Invalid user type");
        }

        ZapMeIdentity identity = user.Identity
            ?? throw new InvalidOperationException("ZapMeIdentity cannot be null.");
        SignInProperties sessionProperties = user.SignInProperties
            ?? throw new InvalidOperationException("SignInProperties cannot be null.");

        if (identity.AuthenticationType != _scheme.Name)
        {
            if (!identity.IsAuthenticated)
            {
                throw new ArgumentException("The identity must be authenticated.", nameof(claimsIdentity));
            }

            throw new ArgumentException("The identity must be authenticated using the same scheme.", nameof(claimsIdentity));
        }


        TimeSpan expiresIn = _options.ExpiresTimeSpanSession;
        if (sessionProperties.RememberMe)
        {
            expiresIn = _options.ExpiresTimeSpanRemembered;
        }
        DateTime expiresAt = DateTime.UtcNow.Add(expiresIn);

        SessionEntity session = await _sessionStore.TryCreateAsync(identity.UserId, sessionProperties.SessionName, expiresAt, _context.RequestAborted)
            ?? throw new InvalidOperationException("Session cannot be null.");

        Response.StatusCode = StatusCodes.Status200OK;
        Response.Cookies.Append(
            CookieName,
            session.Id.ToString(),
            _options.Cookie.Build(_context)
        );
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        Response.Cookies.Delete(CookieName, _options.Cookie.Build(_context));

        return Task.CompletedTask;
    }

    private async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(Constants.LoginCookieName, out string? sessionIdString))
            return AuthenticateResult.NoResult();

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

        ZapMePrincipal principal = new ZapMePrincipal(session.User);

        principal.SessionEntity = session;

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
