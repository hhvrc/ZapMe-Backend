using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeAuthenticationIServiceCollectionExtensions
{
    private static string AuthScheme => ZapMeAuthenticationDefaults.AuthenticationScheme;

    public static void AddZapMeAuthentication([NotNull] this IServiceCollection services, Action<ZapMeAuthenticationOptions>? configureOptions = null)
    {
        services.Configure<AuthenticationOptions>(o =>
        {
            o.AddScheme(AuthScheme, scheme =>
            {
                scheme.HandlerType = typeof(ZapMeAuthenticationHandler);
                scheme.DisplayName = null; // TODO: changeme
            });
        });

        if (configureOptions != null)
        {
            services.Configure(AuthScheme, configureOptions);
        }

        services.AddOptions<ZapMeAuthenticationOptions>(AuthScheme).Validate(o =>
        {
            o.Validate(AuthScheme);
            return true;
        });

        services.AddTransient<IAuthenticationSignInHandler, ZapMeAuthenticationHandler>();
    }
}
