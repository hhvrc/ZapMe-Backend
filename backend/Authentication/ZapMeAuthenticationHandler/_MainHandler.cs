using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ZapMe.Authentication.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Options;

namespace ZapMe.Authentication;

public sealed partial class ZapMeAuthenticationHandler : IAuthenticationSignInHandler
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
    private IServiceProvider ServiceProvider => _context.RequestServices;
    private string RequestingIpAddress => _context.GetRemoteIP();
    private string RequestingIpCountry => _context.GetCloudflareIPCountry();
    private string RequestingUserAgent => _context.GetRemoteUserAgent();
    private CancellationToken CancellationToken => _context.RequestAborted;

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _options = _optionsMonitor.CurrentValue;

        return Task.CompletedTask;
    }

    public Task SignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        string? authScheme = claimsIdentity.Identity?.AuthenticationType;
        if (String.IsNullOrEmpty(authScheme))
        {
            _logger.LogError($"Cannot sign in with an empty AuthenticationScheme.");
            return CreateHttpError.InternalServerError().Write(Response);
        }

        if (authScheme == _scheme.Name)
        {
            return ZapMeSignInAsync(claimsIdentity, properties);
        }

        return SignInOAuthAsync(authScheme, claimsIdentity, properties);
    }
    private Task FinishSignInAsync(SessionEntity session)
    {
        Response.StatusCode = StatusCodes.Status200OK;
        return Response.WriteAsJsonAsync(new SessionDto(session), _jsonSerializerOptions);
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        return Task.CompletedTask;
    }

    private async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        DateTime requestTime = DateTime.UtcNow;

        StringValues authHeader = Request.Headers.Authorization;
        if (!authHeader.Any())
        {
            return AuthenticateResult.NoResult();
        }

        if (
            !AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out AuthenticationHeaderValue? authHeaderValue) ||
            !String.Equals(authHeaderValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
           )
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }

        if (!Guid.TryParse(authHeaderValue.Parameter, out Guid sessionId))
            return AuthenticateResult.Fail("Malformed Login Cookie");

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
            .AsSplitQuery() // Performance improvement suggested by EF Core
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ExpiresAt > requestTime, CancellationToken);
        if (session == null)
        {
            return AuthenticateResult.Fail("Invalid or Expired Login Cookie");
        }

        if (!session.User!.EmailVerified)
        {
            return AuthenticateResult.Fail("Email is not verified");
        }

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

    // Calling Authenticate more than once should always return the original value.
    public Task<AuthenticateResult> AuthenticateAsync() => _authenticateTask ??= HandleAuthenticateAsync();

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        return CreateHttpError.Unauthorized().Write(Response);
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        return CreateHttpError.Forbidden().Write(Response);
    }
}