using OneOf;
using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;

namespace ZapMe.Authentication;

public sealed record OAuthProviderVariables(string ProviderName, string ProviderUserId, string ProviderUserName, string ProviderUserEmail, bool ProviderUserEmailVerified, string? ProfilePictureUrl);
public static class OAuthClaimsFetchers
{
    public static OneOf<OAuthProviderVariables, ErrorDetails> FetchClaims(string authScheme, ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        return authScheme.ToLower() switch
        {
            AuthSchemes.Discord => FetchDiscordClaims(claimsPrincipal, logger),
            AuthSchemes.GitHub => FetchGithubClaims(claimsPrincipal, logger),
            AuthSchemes.Twitter => FetchTwitterClaims(claimsPrincipal, logger),
            AuthSchemes.Google => FetchGoogleClaims(claimsPrincipal, logger),
            _ => OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(HttpErrors.UnsupportedSSOProvider(authScheme)),
        };
    }
    private static OneOf<OAuthProviderVariables, ErrorDetails> FetchDiscordClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? discordId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? discordName = claimsPrincipal.FindFirst("urn:discord:name")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? discordEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        bool discordEmailVerified = claimsPrincipal.FindFirst("urn:discord:verified")?.Value?.ToLowerInvariant() == "true";
        string? discordProfilePictureUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(discordId) || String.IsNullOrEmpty(discordName) || String.IsNullOrEmpty(discordEmail))
        {
            logger.LogError("Discord OAuth claims are missing");
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }

        return new OAuthProviderVariables(AuthSchemes.Discord, discordId, discordName, discordEmail, discordEmailVerified, discordProfilePictureUrl);
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
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }

        return new OAuthProviderVariables(AuthSchemes.GitHub, githubId, githubName, githubEmail, false, githubProfilePictureUrl); // GitHub doesn't provide email verification status
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
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }

        return new OAuthProviderVariables(AuthSchemes.Twitter, twitterId, twitterName, twitterEmail, true, twitterProfilePictureUrl); // Twitter will set email to null if it's not verified
    }
    private static OneOf<OAuthProviderVariables, ErrorDetails> FetchGoogleClaims(ClaimsPrincipal claimsPrincipal, ILogger logger)
    {
        string? googleId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? googleName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? googleEmail = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        bool googleEmailVerified = claimsPrincipal.FindFirst("email_verified")?.Value?.ToLowerInvariant() == "true";
        string? googleProfilePictureUrl = claimsPrincipal.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(googleId) || String.IsNullOrEmpty(googleName) || String.IsNullOrEmpty(googleEmail))
        {
            logger.LogError("Google OAuth claims are missing");
            return OneOf<OAuthProviderVariables, ErrorDetails>.FromT1(HttpErrors.InternalServerError);
        }
        return new OAuthProviderVariables(AuthSchemes.Google, googleId, googleName, googleEmail, googleEmailVerified, googleProfilePictureUrl);
    }
}