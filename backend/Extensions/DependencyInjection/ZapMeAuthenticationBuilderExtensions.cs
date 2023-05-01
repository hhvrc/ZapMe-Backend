using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Authentication;
using ZapMe.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddZapMe(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<AuthenticationOptions>(o =>
        {
            o.AddScheme(ZapMeAuthenticationDefaults.AuthenticationScheme, scheme =>
            {
                scheme.HandlerType = typeof(ZapMeAuthenticationHandler);
                scheme.DisplayName = null; // TODO: changeme
            });
        });

        builder.Services.AddOptions<ZapMeAuthenticationOptions>().Bind(configuration.GetRequiredSection(ZapMeAuthenticationOptions.SectionName)).ValidateOnStart();
        builder.Services.AddTransient<IAuthenticationSignInHandler, ZapMeAuthenticationHandler>();
        return builder;
    }
}