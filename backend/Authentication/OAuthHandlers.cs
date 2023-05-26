using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Security.Claims;
using System.Threading;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Authentication;

public static class OAuthHandlers
{
    public readonly record struct AuthParameters(string Name, string Email, string? ProfilePictureUrl, string Provider, string ProviderId);
    public readonly record struct AuthenticationEntities(UserEntity User, OAuthConnectionEntity Connection, SessionEntity Session);

    public static OneOf<AuthParameters, ErrorDetails> FetchAuthParams(string authenticationType, ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ILogger logger)
    {
        return authenticationType switch
        {
            "github" => FetchGithubAuthParams(claimsIdentity, properties, logger),
            "twitter" => FetchTwitterAuthParams(claimsIdentity, properties, logger),
            "discord" => FetchDiscordAuthParams(claimsIdentity, properties, logger),
            _ => OneOf<AuthParameters, ErrorDetails>.FromT1(CreateHttpError.UnsupportedOAuthProvider(authenticationType)),
        };
    }

    private static OneOf<AuthParameters, ErrorDetails> FetchGithubAuthParams(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ILogger logger)
    {
        string? githubId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? githubName = claimsIdentity.FindFirst("urn:github:name")?.Value ?? claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        string? githubEmail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
        string? githubProfilePictureUrl = claimsIdentity.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(githubId) || String.IsNullOrEmpty(githubName) || String.IsNullOrEmpty(githubEmail))
        {
            logger.LogError("GitHub OAuth claims are missing");
            return OneOf<AuthParameters, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthParameters(githubName, githubEmail, githubProfilePictureUrl, "github", githubId);
    }
    private static OneOf<AuthParameters, ErrorDetails> FetchDiscordAuthParams(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ILogger logger)
    {
        string? discordId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? discordName = claimsIdentity.FindFirst("urn:discord:name")?.Value ?? claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        string? discordEmail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
        string? discordProfilePictureUrl = claimsIdentity.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(discordId) || String.IsNullOrEmpty(discordName) || String.IsNullOrEmpty(discordEmail))
        {
            logger.LogError("Discord OAuth claims are missing");
            return OneOf<AuthParameters, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthParameters(discordName, discordEmail, discordProfilePictureUrl, "discord", discordId);
    }
    private static OneOf<AuthParameters, ErrorDetails> FetchTwitterAuthParams(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ILogger logger)
    {
        // TODO: invalid claim names, fixme
        string? twitterId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? twitterName = claimsIdentity.FindFirst("urn:twitter:name")?.Value ?? claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        string? twitterEmail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
        string? twitterProfilePictureUrl = claimsIdentity.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value?.Replace("_normal", "_400x400");
        if (String.IsNullOrEmpty(twitterId) || String.IsNullOrEmpty(twitterName) || String.IsNullOrEmpty(twitterEmail))
        {
            logger.LogError("Twitter OAuth claims are missing");
            return OneOf<AuthParameters, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthParameters(twitterName, twitterEmail, twitterProfilePictureUrl, "twitter", twitterId);
    }
    
    public static async Task<OneOf<OAuthConnectionEntity, ErrorDetails>> GetOrCreateConnection(AuthParameters authParams, IServiceProvider serviceProvider, ZapMeContext dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        var connectionEntity = await dbContext.OAuthConnections
            .Include(c => c.User)
            .ThenInclude(u => u.ProfilePicture)
            .FirstOrDefaultAsync(c => c.ProviderName == authParams.Provider && c.ProviderId == authParams.ProviderId, cancellationToken);
        if (connectionEntity != null)
        {
            return connectionEntity;
        }

        ErrorDetails errorDetails;
        ImageEntity? profilePicture = null;
        if (!String.IsNullOrEmpty(authParams.ProfilePictureUrl))
        {
            IImageManager imageManager = serviceProvider.GetRequiredService<IImageManager>();
            var createImageResult = await imageManager.GetOrCreateRecordAsync(authParams.ProfilePictureUrl, "regionThingy", null, null, cancellationToken); // TODO: URGENT: fixme
            if (createImageResult.TryPickT1(out errorDetails, out profilePicture))
            {
                return errorDetails;
            }
        }

        IOAuthConnectionManager connectionManager = serviceProvider.GetRequiredService<IOAuthConnectionManager>();
        var getOrCreateConnectionResult = await connectionManager.GetOrCreateConnectionAsync(authParams.Name, authParams.Email, profilePicture, authParams.Provider, authParams.ProviderId, cancellationToken);
        if (getOrCreateConnectionResult.TryPickT1(out errorDetails, out connectionEntity))
        {
            return errorDetails;
        }

        return connectionEntity;
    }
}
