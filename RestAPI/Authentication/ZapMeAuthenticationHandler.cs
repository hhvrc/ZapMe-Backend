using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationHandler : IAuthenticationSignInHandler
{
    private AuthenticationScheme _scheme = default!;
    private HttpContext _context = default!;
    private Task<AuthenticateResult>? _authenticateTask = null;
    private readonly DatabaseContext _dbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<ZapMeAuthenticationHandler> _logger;

    public ZapMeAuthenticationHandler(DatabaseContext dbContext, IOptions<JsonOptions> options, ILogger<ZapMeAuthenticationHandler> logger)
    {
        _dbContext = dbContext;
        _jsonSerializerOptions = options.Value.JsonSerializerOptions;
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

        if (_scheme.Name != AuthSchemes.Main) throw new ArgumentException($"Invalid scheme: {_scheme.Name}");

        return Task.CompletedTask;
    }

    public async Task SignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        string? authScheme = claimsIdentity.Identity?.AuthenticationType;
        if (String.IsNullOrEmpty(authScheme))
        {
            _logger.LogError($"Cannot sign in with an empty AuthenticationScheme.");
            await HttpErrors.Unauthorized.Write(Response);
            return;
        }

        ErrorDetails errorDetails;
        SessionEntity? session;
        if (authScheme == AuthSchemes.Main)
        {
            session = await claimsIdentity.GetSessionAsync(_dbContext, CancellationToken);
        }
        else
        {
            // Fetch the claims provided by the OAuth provider
            var fetchClaimsResult = OAuthClaimsFetchers.FetchClaims(authScheme, claimsIdentity, _logger);
            if (fetchClaimsResult.TryPickT1(out errorDetails, out SSOProviderData? ssoProviderData))
            {
                await errorDetails.Write(Response, _jsonSerializerOptions);
                return;
            }

            // Try to fetch the user's existing SSO connection
            SSOConnectionEntity? connectionEntity = await _dbContext.SSOConnections.Include(c => c.User).ThenInclude(u => u.ProfilePicture)
                .FirstOrDefaultAsync(c => c.ProviderName == ssoProviderData.ProviderName && c.ProviderUserId == ssoProviderData.ProviderUserId, CancellationToken);
            if (connectionEntity == null)
            {
                string token = await ServiceProvider.GetRequiredService<ISSOStateStore>().InsertProviderDataAsync(
                    RequestingIpAddress,
                    ssoProviderData,
                    CancellationToken
                );

                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Location = QueryHelpers.AddQueryString($"{App.WebsiteUrl}/register", "ssoToken", token);
                await Response.WriteAsync("");
                return;
            }

            // Create a new session for the user
            session = await ServiceProvider.GetRequiredService<ISessionManager>().CreateAsync(
                connectionEntity.User.Id,
                RequestingIpAddress,
                RequestingIpCountry,
                RequestingUserAgent,
                true, // TODO: pass in a parameter to determine if the session should be persistent
                CancellationToken
            );
        }

        if (session == null)
        {
            _logger.LogError($"Cannot sign in with an empty session.");
            await HttpErrors.Unauthorized.Write(Response);
            return;
        }

        Response.StatusCode = StatusCodes.Status200OK;
        await Response.WriteAsJsonAsync(session.ToDto(), _jsonSerializerOptions);
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

        SessionEntity? session = await _dbContext.Sessions.AsTracking()
            .Include(s => s.User)
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

        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(session.ToClaimsIdentity()), AuthSchemes.Main));
    }

    // Calling Authenticate more than once should always return the original value.
    public Task<AuthenticateResult> AuthenticateAsync() => _authenticateTask ??= HandleAuthenticateAsync();

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        return HttpErrors.Unauthorized.Write(Response);
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        return HttpErrors.Forbidden.Write(Response);
    }
}