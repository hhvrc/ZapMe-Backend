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
            var tempDataStore = _context.RequestServices.GetRequiredService<ITemporaryDataStore>();

            var cacheKey = "oauthticket:" + HashingUtils.Sha256_Base64(oauthClaims.Provider + oauthClaims.ProviderId);
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(15);
            await tempDataStore.SetAsync(
                cacheKey,
                oauthClaims,
                expiresAt,
                CancellationToken
            );

            // If the user doesn't exist or doesn't have an OAuth connection, inform the client to create a new account using the /api/v1/auth/o/create endpoint
            await WriteOkJsonResponse(new OAuthResult
            {
                ResultType = OAuthResultType.RequireAccountCreation,
                OAuthTicket = new OAuthTicket(cacheKey, expiresAt)
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
}