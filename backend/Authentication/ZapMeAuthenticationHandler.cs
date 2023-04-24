using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using ZapMe.Authentication.Models;
using ZapMe.Data;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationHandler : IAuthenticationSignInHandler
{
    private AuthenticationScheme _scheme = default!;
    private HttpContext _context = default!;
    private ZapMeAuthenticationOptions _options = default!;
    private Task<AuthenticateResult>? _authenticateTask = null;
    private readonly ZapMeContext _dbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IOptionsMonitor<ZapMeAuthenticationOptions> _optionsMonitor;
    private readonly ILogger<ZapMeAuthenticationHandler> _logger;

    public ZapMeAuthenticationHandler(ZapMeContext dbContext, IOptions<JsonOptions> options, IOptionsMonitor<ZapMeAuthenticationOptions> optionsMonitor, ILogger<ZapMeAuthenticationHandler> logger)
    {
        _dbContext = dbContext;
        _jsonSerializerOptions = options.Value.JsonSerializerOptions;
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

        SignInOk result = new SignInOk(session);

        Response.StatusCode = StatusCodes.Status200OK;
        Response.Cookies.Append(
            _options.CookieName,
            session.Id.ToString(),
            new CookieOptions
            {
                Path = "/",
#if DEBUG
                SameSite = SameSiteMode.None,
#else
                SameSite = SameSiteMode.Strict,
#endif
                HttpOnly = true,
                MaxAge = session.ExpiresAt - DateTime.UtcNow,
                IsEssential = true,
                Secure = true
            }
        );

        await Response.WriteAsJsonAsync(result, _jsonSerializerOptions);
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        Response.Cookies.Delete(_options.CookieName);

        return Task.CompletedTask;
    }

    private async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var requestTime = DateTime.UtcNow;

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

        // TODO: Consider doing JWTs instead, and then fetching the user from controllers and check for session expiration there
        SessionEntity? session = await _dbContext.Sessions.AsTracking()
            .Include(s => s.User).ThenInclude(u => u!.ProfilePicture)
            .Include(s => s.User).ThenInclude(u => u!.Sessions)
            .Include(s => s.User).ThenInclude(u => u!.LockOuts)
            .Include(s => s.User).ThenInclude(u => u!.UserRoles)
            .Include(s => s.User).ThenInclude(u => u!.Relations)
            .Include(s => s.User).ThenInclude(u => u!.FriendRequestsOutgoing)
            .Include(s => s.User).ThenInclude(u => u!.FriendRequestsIncoming)
            .Include(s => s.User).ThenInclude(u => u!.OauthConnections)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ExpiresAt > requestTime, cancellationToken);
        if (session == null)
            return AuthenticateResult.Fail("Invalid or Expired Login Cookie");

        TimeSpan lifeTime = session.ExpiresAt - session.CreatedAt;
        DateTime halfLife = session.CreatedAt + (lifeTime / 2);

        // TODO: is this good or bad?
        // Sliding window refresh
        if (requestTime > halfLife)
        {
            // refresh session
            session.ExpiresAt = requestTime.Add(lifeTime);
            await _dbContext.SaveChangesAsync();
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