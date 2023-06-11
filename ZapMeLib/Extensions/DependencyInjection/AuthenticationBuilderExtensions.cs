using Microsoft.AspNetCore.Authentication;
using ZapMe.Authentication;
using ZapMe.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddZapMe(this AuthenticationBuilder builder)
    {
        builder.Services.AddOptions<AuthenticationOptions>().Configure(o =>
        {
            o.AddScheme(AuthSchemes.Main, scheme =>
            {
                scheme.HandlerType = typeof(ZapMeAuthenticationHandler);
                scheme.DisplayName = null; // TODO: changeme
            });
        });

        builder.Services.AddTransient<IAuthenticationSignInHandler, ZapMeAuthenticationHandler>();
        return builder;
    }
}