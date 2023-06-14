using Microsoft.EntityFrameworkCore;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Utils;

namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetSessionId(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal.FindFirst(ZapMeClaimTypes.SessionId)?.Value ?? throw new NullReferenceException(ZapMeClaimTypes.SessionId + " claim not found"));
    }
    public static Guid? TryGetSessionId(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.SessionId);
        if (claim is null) return null;

        return Guid.Parse(claim.Value);
    }

    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal.FindFirst(ZapMeClaimTypes.UserId)?.Value ?? throw new NullReferenceException(ZapMeClaimTypes.UserId + " claim not found"));
    }
    public static Guid? TryGetUserId(this ClaimsPrincipal principal)
    {
        Claim? claim = principal.FindFirst(ZapMeClaimTypes.UserId);
        if (claim is null) return null;

        return Guid.Parse(claim.Value);
    }

    public static string GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ZapMeClaimTypes.UserName)?.Value ?? throw new NullReferenceException(ZapMeClaimTypes.UserName + " claim not found");
    }

    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ZapMeClaimTypes.UserEmail)?.Value ?? throw new NullReferenceException(ZapMeClaimTypes.UserEmail + " claim not found");
    }

    public static bool GetUserEmailVerified(this ClaimsPrincipal principal)
    {
        return Boolean.Parse(principal.FindFirst(ZapMeClaimTypes.UserEmailVerified)?.Value ?? throw new NullReferenceException(ZapMeClaimTypes.UserEmailVerified + " claim not found"));
    }

    public static async Task<SessionEntity?> TryGetSessionAsync(this ClaimsPrincipal principal, DatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        Guid? sessionId = TryGetSessionId(principal);
        if (!sessionId.HasValue) return null;

        return await dbContext.Sessions.Where(u => u.Id == sessionId).SingleOrDefaultAsync(cancellationToken);
    }

    public static async Task<UserEntity?> TryGetUserAsync(this ClaimsPrincipal principal, DatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        Guid? userId = TryGetUserId(principal);
        if (!userId.HasValue) return null;

        return await dbContext.Users.Where(u => u.Id == userId).SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Will only return a user if the password is correct.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="password"></param>
    /// <param name="dbContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<UserEntity?> VerifyUserPasswordAsync(this ClaimsPrincipal principal, string password, DatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        var user = await TryGetUserAsync(principal, dbContext, cancellationToken);
        if (user is null) return null;

        return PasswordUtils.VerifyPassword(password, user.PasswordHash) ? user : null;
    }
}
