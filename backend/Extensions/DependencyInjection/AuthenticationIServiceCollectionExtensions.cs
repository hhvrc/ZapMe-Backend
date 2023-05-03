using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationIServiceCollectionExtensions
{
    public static void ZMAddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(ZapMeAuthenticationDefaults.AuthenticationScheme)
            .AddZapMe(configuration)
            //.AddDiscord()
            //.AddGitHub()
            //.AddTwitter()
            //.AddGoogle()
            ;
    }
}
