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
            .AddGoogle(opt =>
            {
                opt.ClientId = configuration["Authorization:Google:ClientId"]!;
                opt.ClientSecret = configuration["Authorization:Google:ClientSecret"]!;
                opt.CallbackPath = configuration["Authorization:Google:CallbackPath"]!;
                opt.Scope.Add("openid");
                opt.Scope.Add(".../auth/userinfo.email");
            })
            .AddGitHub(opt =>
            {
                opt.ClientId = configuration["Authorization:GitHub:ClientId"]!;
                opt.ClientSecret = configuration["Authorization:GitHub:ClientSecret"]!;
                opt.CallbackPath = configuration["Authorization:GitHub:CallbackPath"]!;
                opt.Scope.Add("user:email");
            });/*
            .AddTwitter(options => {
            });
            */

    }
}
