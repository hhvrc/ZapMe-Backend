﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;

namespace ZapMe.Authentication;

public partial class ZapMeAuthenticationHandler
{
    private async Task SignInSSOAsync(string authScheme, ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        ErrorDetails errorDetails;

        // Fetch the claims provided by the OAuth provider
        var fetchClaimsResult = OAuthClaimsFetchers.FetchClaims(authScheme, claimsIdentity, _logger);
        if (fetchClaimsResult.TryPickT1(out errorDetails, out SSOProviderData? ssoProviderData))
        {
            await errorDetails.Write(Response, _jsonSerializerOptions);
            return;
        }

        // Try to fetch the user's existing SSO connection
        var connectionEntity = await _dbContext.SSOConnections
            .Include(c => c.User)
            .ThenInclude(u => u.ProfilePicture)
            .FirstOrDefaultAsync(c => c.ProviderName == ssoProviderData.ProviderName && c.ProviderUserId == ssoProviderData.ProviderUserId, CancellationToken);
        if (connectionEntity == null)
        {
            var stateStore = ServiceProvider.GetRequiredService<ISSOStateStore>();

            var expiresAt = DateTime.UtcNow + SSOConstants.StateLifetime;
            var token = await stateStore.InsertProviderDataAsync(
                RequestingIpAddress,
                ssoProviderData,
                CancellationToken
            );

            Response.StatusCode = StatusCodes.Status302Found;
            Response.Headers.Location = QueryHelpers.AddQueryString($"{App.WebsiteUrl}/register", "ssoToken", token);
            await Response.StartAsync(CancellationToken);
            return;
        }

        // Create a new session for the user
        var sessionManager = ServiceProvider.GetRequiredService<ISessionManager>();
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