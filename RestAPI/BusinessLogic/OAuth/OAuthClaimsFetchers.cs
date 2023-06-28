using OneOf;
using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Helpers;

namespace ZapMe.BusinessLogic.OAuth;

public static class OAuthClaimsFetchers
{
    public static OneOf<SSOProviderData, ErrorDetails> FetchClaims(string authScheme, ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        return authScheme.ToLower() switch
        {
            AuthenticationConstants.DiscordScheme => FetchDiscordClaims(claimsPrincipal, logger),
            AuthenticationConstants.GitHubScheme => FetchGithubClaims(claimsPrincipal, logger),
            AuthenticationConstants.TwitterScheme => FetchTwitterClaims(claimsPrincipal, logger),
            AuthenticationConstants.GoogleScheme => FetchGoogleClaims(claimsPrincipal, logger),
            _ => OneOf<SSOProviderData, ErrorDetails>.FromT1(HttpErrors.UnsupportedSSOProvider(authScheme)),
        };
    }
    private static OneOf<SSOProviderData, ErrorDetails> FetchDiscordClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? discordId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? discordName = claimsPrincipal.FindFirst("urn:discord:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? discordEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        bool discordEmailVerified = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserEmailVerified)?.Value?.ToLowerInvariant() == "true";
        string? discordAvatarUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserAvatarUrl)?.Value;
        string? discordBannerUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserBannerUrl)?.Value;
        if (String.IsNullOrEmpty(discordId) || String.IsNullOrEmpty(discordName) || String.IsNullOrEmpty(discordEmail))
        {
            logger.LogError("Discord OAuth claims are missing");
            return OneOf<SSOProviderData, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }

        return new SSOProviderData(AuthenticationConstants.DiscordScheme, discordId, discordName, discordEmail, discordEmailVerified, discordAvatarUrl, discordBannerUrl);
    }
    private static OneOf<SSOProviderData, ErrorDetails> FetchGithubClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? githubId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? githubName = claimsPrincipal.FindFirst("urn:github:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? githubEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        string? githubAvatarUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserAvatarUrl)?.Value;
        if (String.IsNullOrEmpty(githubId) || String.IsNullOrEmpty(githubName) || String.IsNullOrEmpty(githubEmail))
        {
            logger.LogError("GitHub OAuth claims are missing");
            return OneOf<SSOProviderData, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }

        return new SSOProviderData(AuthenticationConstants.GitHubScheme, githubId, githubName, githubEmail, false, githubAvatarUrl, null); // GitHub doesn't provide email verification status
    }
    private static OneOf<SSOProviderData, ErrorDetails> FetchTwitterClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? twitterId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? twitterName = claimsPrincipal.FindFirst("urn:twitter:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? twitterEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        string? twitterAvatarUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserAvatarUrl)?.Value?.Replace("_normal", "_400x400");
        string? twitterBannerUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserBannerUrl)?.Value;
        if (String.IsNullOrEmpty(twitterId) || String.IsNullOrEmpty(twitterName) || String.IsNullOrEmpty(twitterEmail))
        {
            logger.LogError("Twitter OAuth claims are missing");
            return OneOf<SSOProviderData, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }

        return new SSOProviderData(AuthenticationConstants.TwitterScheme, twitterId, twitterName, twitterEmail, true, twitterAvatarUrl, twitterBannerUrl); // Twitter will set email to null if it's not verified
    }
    private static OneOf<SSOProviderData, ErrorDetails> FetchGoogleClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? googleId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? googleName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? googleEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        bool googleEmailVerified = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserEmailVerified)?.Value?.ToLowerInvariant() == "true";
        string? googleAvatarUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.UserAvatarUrl)?.Value;
        if (String.IsNullOrEmpty(googleId) || String.IsNullOrEmpty(googleName) || String.IsNullOrEmpty(googleEmail))
        {
            logger.LogError("Google OAuth claims are missing");
            return OneOf<SSOProviderData, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }
        return new SSOProviderData(AuthenticationConstants.GoogleScheme, googleId, googleName, googleEmail, googleEmailVerified, googleAvatarUrl, null);
    }
}