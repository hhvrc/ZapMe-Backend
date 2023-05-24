using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ZapMe.Data;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

public static class OAuthHandlers
{
    internal static Task<SessionEntity> HandleSignInAsync(string authenticationType, ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ZapMeContext dbContext, ILogger<ZapMeAuthenticationHandler> logger)
    {
        return authenticationType switch
        {
            "GitHub" => HandleGithubSignInAsync(claimsIdentity, properties, dbContext, logger),
            "Google" => HandleGoogleSignInAsync(claimsIdentity, properties, dbContext, logger),
            "Twitter" => HandleTwitterSignInAsync(claimsIdentity, properties, dbContext, logger),
            "Discord" => HandleDiscordSignInAsync(claimsIdentity, properties, dbContext, logger),
            _ => throw new NotImplementedException(),
        };
    }

    private static async Task<SessionEntity> HandleGithubSignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ZapMeContext dbContext, ILogger<ZapMeAuthenticationHandler> logger)
    {
        Claim? githubIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        Claim? githubHandleClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
        Claim? githubDisplaynameClaim = claimsIdentity.FindFirst("urn:github:name");
        Claim? githubEmailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
        if (githubIdClaim is null || githubHandleClaim is null || githubDisplaynameClaim is null || githubEmailClaim is null)
        {
            logger.LogError("GitHub claims are missing.");
            throw new InvalidOperationException("GitHub claims are missing.");
        }

        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    private static async Task<SessionEntity> HandleDiscordSignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ZapMeContext dbContext, ILogger<ZapMeAuthenticationHandler> logger)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    private static async Task<SessionEntity> HandleTwitterSignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ZapMeContext dbContext, ILogger<ZapMeAuthenticationHandler> logger)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    private static async Task<SessionEntity> HandleGoogleSignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties, ZapMeContext dbContext, ILogger<ZapMeAuthenticationHandler> logger)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
