using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeAuthenticationIServiceCollectionExtensions
{
    public static void AddZapMeAuthentication([NotNull] this IServiceCollection services, Action<ZapMeAuthenticationOptions>? configureOptions = null)
    {
        services.Configure<AuthenticationOptions>(o =>
        {
            o.AddScheme(ZapMeAuthenticationDefaults.AuthenticationScheme, scheme =>
            {
                scheme.HandlerType = typeof(ZapMeAuthenticationHandler);
                scheme.DisplayName = null; // TODO: changeme
            });
        });

        if (configureOptions != null)
        {
            services.Configure(ZapMeAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        services.AddOptions<ZapMeAuthenticationOptions>(ZapMeAuthenticationDefaults.AuthenticationScheme).Validate(o =>
        {
            o.Validate(ZapMeAuthenticationDefaults.AuthenticationScheme);
            return true;
        });

        services.AddTransient<IAuthenticationSignInHandler, ZapMeAuthenticationHandler>();
    }
}