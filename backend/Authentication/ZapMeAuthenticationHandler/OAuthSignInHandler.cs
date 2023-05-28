using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OneOf;
using System.Security.Claims;
using System.Text.Json;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Authentication;

public partial class ZapMeAuthenticationHandler
{
    private async Task SignInOAuthAsync(string authScheme, ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        ErrorDetails errorDetails;

        // Fetch the claims provided by the OAuth provider
        var fetchClaimsResult = OAuthClaimsFetchers.FetchClaims(authScheme, claimsIdentity, _logger);
        if (fetchClaimsResult.TryPickT1(out errorDetails, out var oauthClaims))
        {
            await errorDetails.Write(Response, _jsonSerializerOptions);
            return;
        }

        // Try to fetch the user's existing OAuth connection
        var connectionEntity = await _dbContext.OAuthConnections
            .Include(c => c.User)
            .ThenInclude(u => u.ProfilePicture)
            .FirstOrDefaultAsync(c => c.ProviderName == oauthClaims.Provider && c.ProviderId == oauthClaims.ProviderId, CancellationToken);
        if (connectionEntity == null)
        {
            var distributedCache = _context.RequestServices.GetRequiredService<IDistributedCache>();

            var cacheKey = HashingUtils.Sha256_Base64(oauthClaims.Provider + oauthClaims.ProviderId);
            await distributedCache.SetAsync(
                cacheKey,
                JsonSerializer.SerializeToUtf8Bytes(oauthClaims),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                },
                CancellationToken
            );

            // If the user doesn't exist or doesn't have an OAuth connection, inform the client to create a new account using the /api/v1/auth/o/create endpoint
            await WriteOkJsonResponse(new OAuthResult
            {
                ResultType = OAuthResultType.RequireAccountCreation,
                OAuthTicket = new OAuthTicket(cacheKey, DateTime.UtcNow.AddMinutes(15))
            });
            return;
        }

        // Create a new session for the user
        var sessionManager = _context.RequestServices.GetRequiredService<ISessionManager>();
        var session = await sessionManager.CreateAsync(
            connectionEntity.User,
            RequestingIpAddress,
            RequestingIpCountry,
            RequestingUserAgent,
            true, // TODO: should rememberMe be true?
            CancellationToken
        );

        await FinishSignInAsync(session);
    }

    private readonly record struct AuthenticationEntities(UserEntity User, OAuthConnectionEntity Connection, SessionEntity Session);
    private async Task<OneOf<OAuthConnectionEntity, ErrorDetails>> CreateConnection(OAuthProviderVariables authParams)
    {
        IServiceProvider serviceProvider = _context.RequestServices;
        ErrorDetails errorDetails;
        ImageEntity? profilePicture = null;
        if (!String.IsNullOrEmpty(authParams.ProfilePictureUrl))
        {
            IImageManager imageManager = serviceProvider.GetRequiredService<IImageManager>();
            var createImageResult = await imageManager.GetOrCreateRecordAsync(authParams.ProfilePictureUrl, RequestingIpRegion, null, null, CancellationToken); // TODO: URGENT: fixme
            if (createImageResult.TryPickT1(out errorDetails, out profilePicture))
            {
                return errorDetails;
            }
        }

        IOAuthConnectionManager connectionManager = serviceProvider.GetRequiredService<IOAuthConnectionManager>();
        var getOrCreateConnectionResult = await connectionManager.GetOrCreateConnectionAsync(authParams.Name, authParams.Email, profilePicture, authParams.Provider, authParams.ProviderId, CancellationToken);
        if (getOrCreateConnectionResult.TryPickT1(out errorDetails, out var connectionEntity))
        {
            return errorDetails;
        }

        return connectionEntity!;
    }
}