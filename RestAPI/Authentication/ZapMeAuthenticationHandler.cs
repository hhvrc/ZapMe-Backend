using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationHandler : IAuthenticationSignInHandler
{
    private AuthenticationScheme _scheme = default!;
    private HttpContext _context = default!;
    private Task<AuthenticateResult>? _authenticateTask = null;
    private readonly DatabaseContext _dbContext;
    private readonly ISessionStore _sessionStore;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<ZapMeAuthenticationHandler> _logger;

    public ZapMeAuthenticationHandler(DatabaseContext dbContext, ISessionStore sessionStore, IOptions<JsonOptions> jsonOptions, IOptions<JwtOptions> jwtOptions, ILogger<ZapMeAuthenticationHandler> logger)
    {
        _dbContext = dbContext;
        _sessionStore = sessionStore;
        _jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
        _jwtOptions = jwtOptions.Value;
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

        if (_scheme.Name != AuthenticationConstants.ZapMeScheme) throw new ArgumentException($"Invalid scheme: {_scheme.Name}");

        return Task.CompletedTask;
    }

    public async Task SignInAsync(ClaimsPrincipal claimsPrincipal, AuthenticationProperties? properties)
    {
        string? authScheme = claimsPrincipal.Identity?.AuthenticationType;
        if (String.IsNullOrEmpty(authScheme))
        {
            _logger.LogError($"Cannot sign in with an empty AuthenticationScheme.");
            await HttpErrors.Unauthorized.Write(Response, _jsonSerializerOptions);
            return;
        }

        DateTime issuedAt;
        DateTime expiresAt;
        ClaimsIdentity claimsIdentity;
        if (authScheme != AuthenticationConstants.ZapMeScheme)
        {
            // Fetch the claims provided by the OAuth provider
            var fetchClaimsResult = OAuthClaimsFetchers.FetchClaims(authScheme, claimsPrincipal, _logger);
            if (fetchClaimsResult.TryPickT1(out ErrorDetails errorDetails, out SSOProviderData? ssoProviderData))
            {
                await errorDetails.Write(Response, _jsonSerializerOptions);
                return;
            }

            // Try to fetch the user's existing SSO connection
            SSOConnectionEntity? connectionEntity = await _dbContext.SSOConnections
                .Include(c => c.User).ThenInclude(u => u.ProfileAvatar)
                .Include(c => c.User).ThenInclude(u => u.ProfileBanner)
                .FirstOrDefaultAsync(c => c.ProviderName == ssoProviderData.ProviderName && c.ProviderUserId == ssoProviderData.ProviderUserId, CancellationToken);
            if (connectionEntity is null)
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
            var session = await ServiceProvider.GetRequiredService<ISessionManager>().CreateAsync(
                connectionEntity.User,
                RequestingIpAddress,
                RequestingIpCountry,
                RequestingUserAgent,
                true, // TODO: pass in a parameter to determine if the session should be persistent
                CancellationToken
            );

            if (session is null)
            {
                _logger.LogError($"Cannot sign in with an empty session.");
                await HttpErrors.Unauthorized.Write(Response, _jsonSerializerOptions);
                return;
            }

            issuedAt = session.CreatedAt;
            expiresAt = session.ExpiresAt;
            claimsIdentity = session.ToClaimsIdentity();
        }
        else
        {
            ArgumentNullException.ThrowIfNull(properties);
            issuedAt = properties.IssuedUtc?.UtcDateTime ?? throw new ArgumentNullException(nameof(properties.IssuedUtc));
            expiresAt = properties.ExpiresUtc?.UtcDateTime ?? throw new ArgumentNullException(nameof(properties.ExpiresUtc));
            claimsIdentity = claimsPrincipal.Identities.FirstOrDefault() ?? throw new ArgumentNullException(nameof(claimsPrincipal.Identities));
        }

        Response.StatusCode = StatusCodes.Status200OK;
        await Response.WriteAsJsonAsync(new AuthenticationResponse(JwtToken: JwtTokenUtils.GenerateJwtToken(claimsIdentity, issuedAt, expiresAt, _jwtOptions.SigningKey)));
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
            !String.Equals(authHeaderValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) ||
            String.IsNullOrEmpty(authHeaderValue.Parameter)
           )
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }

        if (!JwtTokenUtils.ValidateJwtToken(authHeaderValue.Parameter, _jwtOptions.SigningKey, out ClaimsPrincipal claimsPrincipal, out SecurityToken securityToken))
        {
            return AuthenticateResult.Fail("Invalid JWT token.");
        }

        if (!claimsPrincipal.GetUserEmailVerified())
        {
            return AuthenticateResult.Fail("Email is not verified.");
        }

        var session = await _sessionStore.TryGetAsync(claimsPrincipal.GetSessionId(), CancellationToken);
        if (session is null)
        {
            return AuthenticateResult.Fail("Invalid or Expired Session");
        }

        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(session.ToClaimsIdentity()), AuthenticationConstants.ZapMeScheme));
    }

    // Calling Authenticate more than once should always return the original value.
    public Task<AuthenticateResult> AuthenticateAsync() => _authenticateTask ??= HandleAuthenticateAsync();

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        return HttpErrors.Unauthorized.Write(Response, _jsonSerializerOptions);
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        return HttpErrors.Forbidden.Write(Response, _jsonSerializerOptions);
    }
}