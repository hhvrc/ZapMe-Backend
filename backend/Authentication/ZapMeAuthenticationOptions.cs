using Microsoft.AspNetCore.Authentication;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationOptions : AuthenticationSchemeOptions
{
    private CookieBuilder _cookieBuilder = new RequestPathBaseCookieBuilder
    {
        SameSite = SameSiteMode.Lax,
        HttpOnly = true,
        SecurePolicy = CookieSecurePolicy.SameAsRequest,
        IsEssential = true,
    };

    public CookieBuilder Cookie { get => _cookieBuilder; set => _cookieBuilder = value ?? throw new NullReferenceException(nameof(value)); }
    public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromDays(30);
    public bool SlidingExpiration { get; set; } = true;
}
