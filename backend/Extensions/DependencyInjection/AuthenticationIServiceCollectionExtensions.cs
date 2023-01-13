using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ZapMe.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationIServiceCollectionExtensions
{
    public static void ZMAddAuthentication([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]!)),
                };
                opt.Events = new JwtBearerEvents
                {
                    OnChallenge = JwtEventHandlers.OnChallenge,
                    OnForbidden = JwtEventHandlers.OnForbidden,
                    OnTokenValidated = JwtEventHandlers.OnTokenValidated,
                    OnAuthenticationFailed = JwtEventHandlers.OnAuthenticationFailed,
                };
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
