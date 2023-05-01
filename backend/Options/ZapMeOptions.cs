using Microsoft.AspNetCore.Authentication;

namespace ZapMe.Options;

public sealed class ZapMeOptions
{
    public const string SectionName = "ZapMe";

    public required ZapMeAuthenticationOptions Authentication { get; set; }
}

public sealed class ZapMeAuthenticationOptions
{
    public const string SectionName = ZapMeOptions.SectionName + ":Authentication";

    /// <summary>
    /// Name of the cookie which will be sent to client
    /// </summary>
    public required string CookieName { get; set; }

    /// <summary>
    /// Sets if the cookie should be renewed if its half way through the expiration time.
    /// </summary>
    public required bool SlidingExpiration { get; set; }
}