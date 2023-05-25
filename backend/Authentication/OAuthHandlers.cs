using Microsoft.AspNetCore.Authentication;
using OneOf;
using System.Security.Claims;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;

namespace ZapMe.Authentication;

public static class OAuthHandlers
{
    public readonly record struct AuthParameters(string Name, string Email, string? ProfilePictureUrl, string Provider, string ProviderId);
    public readonly record struct AuthenticationEntities(UserEntity User, OAuthConnectionEntity Connection, SessionEntity Session);

    public static OneOf<AuthParameters, ErrorDetails> FetchAuthParams(string authenticationType, ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ILogger logger)
    {
        return authenticationType switch
        {
            "GitHub" => FetchGithubAuthParams(claimsIdentity, properties, logger),
            "Google" => FetchGoogleAuthParams(claimsIdentity, properties, logger),
            "Twitter" => FetchTwitterAuthParams(claimsIdentity, properties, logger),
            "Discord" => FetchDiscordAuthParams(claimsIdentity, properties, logger),
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
        // TODO: invalid claim names, fixme
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
        string? twitterProfilePictureUrl = claimsIdentity.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(twitterId) || String.IsNullOrEmpty(twitterName) || String.IsNullOrEmpty(twitterEmail))
        {
            logger.LogError("Twitter OAuth claims are missing");
            return OneOf<AuthParameters, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthParameters(twitterName, twitterEmail, twitterProfilePictureUrl, "twitter", twitterId);
    }
    private static OneOf<AuthParameters, ErrorDetails> FetchGoogleAuthParams(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ILogger logger)
    {
        // TODO: invalid claim names, fixme
        string? googleId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? googleName = claimsIdentity.FindFirst("urn:google:name")?.Value ?? claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        string? googleEmail = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
        string? googleProfilePictureUrl = claimsIdentity.FindFirst(ZapMeClaimTypes.ProfileImage)?.Value;
        if (String.IsNullOrEmpty(googleId) || String.IsNullOrEmpty(googleName) || String.IsNullOrEmpty(googleEmail))
        {
            logger.LogError("Google OAuth claims are missing");
            return OneOf<AuthParameters, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthParameters(googleName, googleEmail, googleProfilePictureUrl, "google", googleId);
    }
    /*
    public static async Task<OneOf<AuthenticationEntities, ErrorDetails>> AuthenticateUser(AuthParameters authParams, IServiceProvider serviceProvider, ZapMeContext dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        OAuthConnectionEntity? connection = await dbContext.OAuthConnections
            .Include(oc => oc.User)
            .ThenInclude(u => u.Sessions)
            .FirstOrDefaultAsync(x => x.ProviderName == authParams.Provider && x.ProviderId == authParams.ProviderId, cancellationToken);
        if (connection is null)
        {
            return await CreateNewUser(authParams, serviceProvider, dbContext, logger);
        }

        return await CreateSession(connection, serviceProvider, dbContext, logger, cancellationToken);
    }

    private static async Task<OneOf<AuthenticationEntities, ErrorDetails>> CreateNewUser(AuthParameters authParams, IServiceProvider serviceProvider, ZapMeContext dbContext, ILogger logger)
    {
        ImageManager imageManager = serviceProvider.GetRequiredService<ImageManager>();
        UserStore userStore = serviceProvider.GetRequiredService<UserStore>();

        UserEntity user = new()
        {
            Name = authParams.Name,
            Email = authParams.Email,
            ProfilePictureUrl = authParams.ProfilePictureUrl,
            Sessions = new List<SessionEntity>(),
        };
        OAuthConnectionEntity connection = new()
        {
            ProviderName = authParams.Provider,
            ProviderId = authParams.ProviderId,
            User = user,
        };
        SessionEntity session = new()
        {
            User = user,
            OAuthConnection = connection,
            Token = Guid.NewGuid().ToString(),
            ExpirationDate = DateTime.UtcNow.AddDays(7),
        };

        try
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.OAuthConnections.AddAsync(connection);
            await dbContext.Sessions.AddAsync(session);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create new user");
            return OneOf<AuthenticationEntities, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthenticationEntities(user, connection, session);
    }

    private static async Task<OneOf<AuthenticationEntities, ErrorDetails>> CreateSession(OAuthConnectionEntity connection, IServiceProvider serviceProvider, ZapMeContext dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        SessionManager sessionManager = serviceProvider.GetRequiredService<SessionManager>();

        sessionManager.CreateAsync(connection.User, cancellationToken);
        SessionEntity session = new()
        {
            User = connection.User,
            OAuthConnection = connection,
            Token = Guid.NewGuid().ToString(),
            ExpirationDate = DateTime.UtcNow.AddDays(7),
        };

        try
        {
            await dbContext.Sessions.AddAsync(session);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create new session");
            return OneOf<AuthenticationEntities, ErrorDetails>.FromT1(CreateHttpError.InternalServerError());
        }

        return new AuthenticationEntities(connection.User, connection, session);
    }
    */
}
