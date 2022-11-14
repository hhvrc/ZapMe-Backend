using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationHandler : AuthenticationHandler<ZapMeAuthenticationOptions>
{
    private readonly ISignInManager _signInManager;

    public ZapMeAuthenticationHandler(
        IOptionsMonitor<ZapMeAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ISignInManager signInManager
        )
        : base(options, logger, encoder, clock)
    {
        _signInManager = signInManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue("access_token", out string? accessToken))
            return AuthenticateResult.NoResult();

        if (String.IsNullOrEmpty(accessToken))
            return AuthenticateResult.Fail("Empty Accesstoken Cookie");

        if (!Guid.TryParse(accessToken, out Guid signInId))
            return AuthenticateResult.Fail("Malformed Accesstoken Cookie");

        SignInEntity? signIn = await _signInManager.TryGetSignInAsync(signInId);
        if (signIn == null)
            return AuthenticateResult.Fail("Invalid Accesstoken Cookie");

        if (!signIn.IsValid)
            return AuthenticateResult.Fail("Expired Accesstoken Cookie");

        Context.SetSignIn(signIn);

        UserEntity user = signIn.User;

        Claim[] claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        ClaimsIdentity identity = new(claims, Scheme.Name);
        GenericPrincipal principal = new(identity, null); // TODO: implement roles
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
