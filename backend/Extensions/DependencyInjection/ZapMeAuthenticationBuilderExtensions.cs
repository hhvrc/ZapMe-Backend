using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddZapMe([NotNull] this AuthenticationBuilder builder, Action<ZapMeAuthenticationOptions>? configureOptions = null)
    {
        builder.Services.AddZapMeAuthentication(configureOptions);

        return builder;
    }
}
