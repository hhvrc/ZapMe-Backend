using Microsoft.AspNetCore.Authentication;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationOptions : AuthenticationSchemeOptions
{
    private CookieBuilder _cookieBuilder = new RequestPathBaseCookieBuilder();

    /// <summary>
    /// Sets if the cookie should be renewed if its half way through the expiration time.
    /// </summary>
    public bool SlidingExpiration { get; set; } = true;

    /// <summary>
    /// Specifies the time span after which the authentication cookie expires if the user does not opt to be remembered.
    /// </summary>
    public TimeSpan ExpiresTimeSpanSession { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Specifies the time span after which the authentication cookie expires if the user opts to be remembered.
    /// </summary>
    public TimeSpan ExpiresTimeSpanRemembered { get; set; } = TimeSpan.FromDays(30);

    /// <summary>
    /// Gets or sets the <see cref="CookieBuilder"/> used to configure the authentication cookie.
    /// </summary>
    public CookieBuilder Cookie { get => _cookieBuilder; set => _cookieBuilder = value ?? throw new NullReferenceException(nameof(value)); }

    public override void Validate()
    {
        base.Validate();

        if (ExpiresTimeSpanRemembered <= TimeSpan.Zero)
        {
            throw new ArgumentException("ExpireTimeSpan must be a positive TimeSpan.");
        }

        if (ExpiresTimeSpanRemembered < Cookie.Expiration)
        {
            throw new ArgumentException("ExpireTimeSpan must be larger than cookie expiration.");
        }

        if (String.IsNullOrEmpty(Cookie.Name))
        {
            throw new ArgumentException("Cookie name must be provided.");
        }

        if (!Cookie.IsEssential)
        {
            throw new ArgumentException("Cookie must be essential.");
        }

        if (Cookie.SameSite != SameSiteMode.Strict)
        {
            throw new ArgumentException("Cookie must be strict.");
        }

        if (Cookie.SecurePolicy != CookieSecurePolicy.Always)
        {
            throw new ArgumentException("Cookie must be secure.");
        }
    }
}
