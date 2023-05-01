using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Authentication;
using ZapMe.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationIServiceCollectionExtensions
{
    public static void ZMAddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(ZapMeAuthenticationDefaults.AuthenticationScheme)
            .AddZapMe(configuration)
            //.AddDiscord()
            .AddGitHub()
            .AddTwitter()
            .AddGoogle()
            ;
    }
}
