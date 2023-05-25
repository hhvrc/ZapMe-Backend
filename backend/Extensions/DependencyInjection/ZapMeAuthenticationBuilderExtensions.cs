using Microsoft.AspNetCore.Authentication;
using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddZapMe(this AuthenticationBuilder builder)
    {
        builder.Services.Configure<AuthenticationOptions>(o =>
        {
            o.AddScheme(ZapMeAuthenticationDefaults.AuthenticationScheme, scheme =>
            {
                scheme.HandlerType = typeof(ZapMeAuthenticationHandler);
                scheme.DisplayName = null; // TODO: changeme
            });
        });

        builder.Services.AddTransient<IAuthenticationSignInHandler, ZapMeAuthenticationHandler>();
        return builder;
    }
}