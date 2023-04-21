using System.Diagnostics.CodeAnalysis;
using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationIServiceCollectionExtensions
{
    public static void ZMAddAuthentication([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services
            .AddAuthentication(ZapMeAuthenticationDefaults.AuthenticationScheme)
            .AddZapMe(opt =>
            {
                opt.CookieName = ZapMeAuthenticationDefaults.AuthenticationScheme + ".Authentication";
                opt.SlidingExpiration = true;
            })
            /*
            .AddDiscord(opt =>
            {
                opt.ClientId = configuration.GetOrThrow("Authorization:Discord:ClientId");
                opt.ClientSecret = configuration.GetOrThrow("Authorization:Discord:ClientSecret");
                opt.CallbackPath = configuration.GetOrThrow("Authorization:Discord:CallbackPath");
            })
            */
            .AddGitHub(opt =>
            {
                opt.ClientId = configuration.GetOrThrow("Authorization:GitHub:ClientId");
                opt.ClientSecret = configuration.GetOrThrow("Authorization:GitHub:ClientSecret");
                opt.CallbackPath = configuration.GetOrThrow("Authorization:GitHub:CallbackPath");
                opt.Scope.Add("user:email");
            })
            /*
            .AddTwitter(options => {
                opt.ClientId = configuration.GetOrThrow("Authorization:Twitter:ClientId");
                opt.ClientSecret = configuration.GetOrThrow("Authorization:Twitter:ClientSecret");
                opt.CallbackPath = configuration.GetOrThrow("Authorization:Twitter:CallbackPath");
            })
            .AddGoogle(opt =>
            {
                opt.ClientId = configuration.GetOrThrow("Authorization:Google:ClientId");
                opt.ClientSecret = configuration.GetOrThrow("Authorization:Google:ClientSecret");
                opt.CallbackPath = configuration.GetOrThrow("Authorization:Google:CallbackPath");
                opt.Scope.Add("openid");
                opt.Scope.Add(".../auth/userinfo.email");
            })
            */
            ;
    }
}
