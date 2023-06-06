using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using ZapMe.Authentication;
using ZapMe.Constants;
using ZapMe.Helpers;
using ZapMe.Options.OAuth;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddZapMe(this AuthenticationBuilder builder)
    {
        builder.Services.AddOptions<AuthenticationOptions>().Configure(o =>
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

    public static AuthenticationBuilder AddOAuthProviders(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        ISecureDataFormat<T> GetSecureDataFormat<T>()
        {
            return new DistributedCacheSecureDataFormat<T>(configuration.GetValue<string>("Redis:ConnectionString")!, TimeSpan.FromMinutes(1));
        }

        return builder.AddDiscord(OAuthConstants.DiscordProviderName, opt =>
        {
            DiscordOAuth2Options discordOptions = DiscordOAuth2Options.Get(configuration);

            opt.ClientId = discordOptions.ClientId;
            opt.ClientSecret = discordOptions.ClientSecret;
            opt.CallbackPath = discordOptions.CallbackPath;
            opt.AccessDeniedPath = discordOptions.AccessDeniedPath;
            foreach (var scope in discordOptions.Scopes) opt.Scope.Add(scope);

            opt.Prompt = "none";
            opt.SaveTokens = true;
            opt.StateDataFormat = GetSecureDataFormat<AuthenticationProperties>();
            opt.CorrelationCookie.HttpOnly = true;
            opt.CorrelationCookie.SameSite = SameSiteMode.None;
            opt.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.ClaimActions.MapCustomJson(ZapMeClaimTypes.ProfileImage, json =>
            {
                string? userId = json.GetString("id");
                string? avatar = json.GetString("avatar");
                if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(avatar))
                    return null;

                return $"https://cdn.discordapp.com/avatars/{userId}/{avatar}.png";
            });
            opt.Validate();
        })
        .AddGitHub(OAuthConstants.GitHubProviderName, opt =>
        {
            GitHubOAuth2Options githubOptions = GitHubOAuth2Options.Get(configuration);

            opt.ClientId = githubOptions.ClientId;
            opt.ClientSecret = githubOptions.ClientSecret;
            opt.CallbackPath = githubOptions.CallbackPath;
            opt.AccessDeniedPath = githubOptions.AccessDeniedPath;
            foreach (var scope in githubOptions.Scopes) opt.Scope.Add(scope);

            opt.SaveTokens = true;
            opt.StateDataFormat = GetSecureDataFormat<AuthenticationProperties>();
            opt.CorrelationCookie.HttpOnly = true;
            opt.CorrelationCookie.SameSite = SameSiteMode.None;
            opt.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.ClaimActions.MapCustomJson(ZapMeClaimTypes.ProfileImage, json =>
            {
                string? avatarUrl = json.GetString("avatar_url");
                string? gravatarId = json.GetString("gravatar_id");

                if (String.IsNullOrEmpty(gravatarId))
                    return avatarUrl;

                if (String.IsNullOrEmpty(avatarUrl))
                    return $"https://www.gravatar.com/avatar/{gravatarId}?s=256";

                return $"https://www.gravatar.com/avatar/{gravatarId}?s=256&d={Uri.EscapeDataString(avatarUrl)}";
            });
            opt.Validate();
        })
        .AddTwitter(OAuthConstants.TwitterProviderName, opt =>
        {
            TwitterOAuth1Options twitterOptions = TwitterOAuth1Options.Get(configuration);

            opt.ConsumerKey = twitterOptions.ConsumerKey;
            opt.ConsumerSecret = twitterOptions.ConsumerSecret;
            opt.CallbackPath = twitterOptions.CallbackPath;
            opt.AccessDeniedPath = twitterOptions.AccessDeniedPath;

            opt.SaveTokens = true;
            opt.RetrieveUserDetails = true;
            opt.StateDataFormat = GetSecureDataFormat<RequestToken>();
            opt.CorrelationCookie.HttpOnly = true;
            opt.CorrelationCookie.SameSite = SameSiteMode.None;
            opt.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.ClaimActions.MapJsonKey(ZapMeClaimTypes.ProfileImage, "profile_image_url_https");
            opt.Validate();
        })
        .AddGoogle(OAuthConstants.GoogleProviderName, opt =>
        {
            GoogleOAuth2Options googleOptions = GoogleOAuth2Options.Get(configuration);

            opt.ClientId = googleOptions.ClientId;
            opt.ClientSecret = googleOptions.ClientSecret;
            opt.CallbackPath = googleOptions.CallbackPath;
            opt.AccessDeniedPath = googleOptions.AccessDeniedPath;
            foreach (var scope in googleOptions.Scopes) opt.Scope.Add(scope);

            opt.SaveTokens = true;
            opt.StateDataFormat = GetSecureDataFormat<AuthenticationProperties>();
            opt.CorrelationCookie.HttpOnly = true;
            opt.CorrelationCookie.SameSite = SameSiteMode.None;
            opt.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.ClaimActions.MapJsonKey(ZapMeClaimTypes.ProfileImage, "picture");
            opt.Validate();
        });
    }
}