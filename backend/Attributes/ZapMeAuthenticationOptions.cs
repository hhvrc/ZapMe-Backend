using Microsoft.AspNetCore.Authentication;

namespace ZapMe.Authentication;

public sealed class ZapMeAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Name of the cookie which will be sent to client
    /// </summary>
    public string CookieName { get; set; } = null!;

    /// <summary>
    /// Sets if the cookie should be renewed if its half way through the expiration time.
    /// </summary>
    public bool SlidingExpiration { get; set; } = true;

    public override void Validate()
    {
        base.Validate();

        if (String.IsNullOrEmpty(CookieName))
        {
            throw new ArgumentException("Cookie name must be provided.");
        }
    }
}