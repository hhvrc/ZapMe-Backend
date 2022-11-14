using ZapMe.Authentication;

namespace Microsoft.AspNetCore.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddZapMe(this AuthenticationBuilder builder, Action<ZapMeAuthenticationOptions>? options = null)
    {
        return builder.AddScheme<ZapMeAuthenticationOptions, ZapMeAuthenticationHandler>(ZapMeAuthenticationDefaults.AuthenticationScheme, options);
    }
}
