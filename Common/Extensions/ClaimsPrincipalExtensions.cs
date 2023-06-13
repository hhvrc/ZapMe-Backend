using Microsoft.EntityFrameworkCore;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Helpers;
using ZapMe.Utils;

namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetSessionId(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.SessionId);

        if (Guid.TryParse(claim?.Value, out Guid sessionId))
        {
            return sessionId;
        }

        return null;
    }

    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.UserId);

        if (Guid.TryParse(claim?.Value, out Guid userId))
        {
            return userId;
        }

        return null;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.UserName);

        return claim?.Value;
    }

    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.UserEmail);

        return claim?.Value;
    }

    public static bool? GetUserEmailVerified(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.UserEmailVerified);

        if (Boolean.TryParse(claim?.Value, out bool emailVerified))
        {
            return emailVerified;
        }

        return null;
    }

    public static async Task<SessionEntity?> GetSessionAsync(this ClaimsPrincipal principal, DatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        Guid? sessionId = GetSessionId(principal);
        if (!sessionId.HasValue) return null;

        return await dbContext.Sessions.Where(u => u.Id == sessionId).SingleOrDefaultAsync(cancellationToken);
    }

    public static async Task<UserEntity?> GetUserAsync(this ClaimsPrincipal principal, DatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        Guid? userId = GetUserId(principal);
        if (!userId.HasValue) return null;

        return await dbContext.Users.Where(u => u.Id == userId).SingleOrDefaultAsync(cancellationToken);
    }

    public static async Task<UserEntity?> CheckPasswordAsync(this ClaimsPrincipal principal, string password, DatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(principal, dbContext, cancellationToken);
        if (user is null) return null;

        return PasswordUtils.CheckPassword(password, user.PasswordHash) ? user : null;
    }
}
