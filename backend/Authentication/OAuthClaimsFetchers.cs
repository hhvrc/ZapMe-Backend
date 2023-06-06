using OneOf;
using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;

namespace ZapMe.Authentication;

public sealed record OAuthProviderVariables(string Name, string Email, string? ProfilePictureUrl, string Provider, string ProviderId);
public static class OAuthClaimsFetchers
{
    public static OneOf<OAuthProviderVariables, ErrorDetails> FetchClaims(string authScheme, ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        return authScheme.ToLower() switch
        {
            OAuthConstants.DiscordProviderName => FetchDiscordClaims(claimsPrincipal, logger),
            OAuthConstants.GitHubProviderName => FetchGithubClaims(claimsPrincipal, logger),
            OAuthConstants.TwitterProviderName => FetchTwitterClaims(claimsPrincipal, logger),
            OAuthConstants.GoogleProviderName => FetchGoogleClaims(claimsPrincipal, logger),
            _ => OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(CreateHttpError.UnsupportedOAuthProvider(authScheme)),
        };
    }
    private static OneOf<OAuthProviderVariables, ErrorDetails> FetchDiscordClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? discordId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? discordName = claimsPrincipal.FindFirst("urn:discord:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? discordEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        string? discordProfilePictureUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(discordId) || String.IsNullOrEmpty(discordName) || String.IsNullOrEmpty(discordEmail))
        {
            logger.LogError("Discord OAuth claims are missing");
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new OAuthProviderVariables(discordName, discordEmail, discordProfilePictureUrl, OAuthConstants.DiscordProviderName, discordId);
    }
    private static OneOf<OAuthProviderVariables, ErrorDetails> FetchGithubClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? githubId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? githubName = claimsPrincipal.FindFirst("urn:github:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? githubEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        string? githubProfilePictureUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(githubId) || String.IsNullOrEmpty(githubName) || String.IsNullOrEmpty(githubEmail))
        {
            logger.LogError("GitHub OAuth claims are missing");
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new OAuthProviderVariables(githubName, githubEmail, githubProfilePictureUrl, OAuthConstants.GitHubProviderName, githubId);
    }
    private static OneOf<OAuthProviderVariables, ErrorDetails> FetchTwitterClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? twitterId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? twitterName = claimsPrincipal.FindFirst("urn:twitter:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? twitterEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        string? twitterProfilePictureUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value?.Replace("_normal", "_400x400");
        if (String.IsNullOrEmpty(twitterId) || String.IsNullOrEmpty(twitterName) || String.IsNullOrEmpty(twitterEmail))
        {
            logger.LogError("Twitter OAuth claims are missing");
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new OAuthProviderVariables(twitterName, twitterEmail, twitterProfilePictureUrl, OAuthConstants.TwitterProviderName, twitterId);
    }
    private static OneOf<OAuthProviderVariables, ErrorDetails> FetchGoogleClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? googleId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? googleName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? googleEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        string? googleProfilePictureUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(googleId) || String.IsNullOrEmpty(googleName) || String.IsNullOrEmpty(googleEmail))
        {
            logger.LogError("Google OAuth claims are missing");
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }
        return new OAuthProviderVariables(googleName, googleEmail, googleProfilePictureUrl, OAuthConstants.GoogleProviderName, googleId);
    }
}