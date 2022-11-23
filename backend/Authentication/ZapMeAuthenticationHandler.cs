using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationHandler : IAuthenticationSignInHandler
{
    private AuthenticationScheme _scheme = default!;
    private HttpContext _context = default!;
    private ZapMeAuthenticationOptions _options = default!;
    private Task<AuthenticateResult>? _authenticateTask = null;
    private readonly ISignInManager _signInManager;
    private readonly IOptionsMonitor<ZapMeAuthenticationOptions> _optionsMonitor;
    private readonly ILogger<ZapMeAuthenticationHandler> _logger;

    public ZapMeAuthenticationHandler(ISignInManager signInManager, IOptionsMonitor<ZapMeAuthenticationOptions> optionsMonitor, ILogger<ZapMeAuthenticationHandler> logger)
    {
        _signInManager = signInManager;
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

    private async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(Constants.LoginCookieName, out string? accessToken))
            return AuthenticateResult.NoResult();

        if (String.IsNullOrEmpty(accessToken))
            return AuthenticateResult.Fail("Empty Login Cookie");

        if (!Guid.TryParse(accessToken, out Guid signInId))
            return AuthenticateResult.Fail("Malformed Login Cookie");

        SignInEntity? signIn = await _signInManager.TryGetSignInAsync(signInId);
        if (signIn == null)
            return AuthenticateResult.Fail("Invalid Login Cookie");

        if (!signIn.IsValid)
            return AuthenticateResult.Fail("Expired Login Cookie");

        _context.SetSignIn(signIn);

        AccountEntity user = signIn.User;

        Claim[] claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        ClaimsIdentity identity = new(claims, _scheme.Name);
        GenericPrincipal principal = new(identity, null); // TODO: implement roles
        AuthenticationTicket ticket = new(principal, _scheme.Name);

        return AuthenticateResult.Success(ticket);
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

    public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        return Task.CompletedTask;
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        return Task.CompletedTask;
    }

    public Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        return Task.CompletedTask;
    }

}
