using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Utils;

namespace ZapMe.BusinessLogic.Users;

public static class PasswordLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns><see langword="true"/> if the password was changed, <see langword="false"/> if the user was not found</returns>
    public static async Task<bool> ChangePassword(DatabaseContext dbContext, Guid userId, string newPassword, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.PasswordHash = PasswordUtils.HashPassword(newPassword);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public enum ChangePasswordWithVerificationResult
    {
        Success,
        UserNotFound,
        InvalidPassword
    }
    public static async Task<ChangePasswordWithVerificationResult> ChangePassword_WithVerification(DatabaseContext dbContext, Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            return ChangePasswordWithVerificationResult.UserNotFound;
        }

        if (!PasswordUtils.VerifyPassword(currentPassword, user.PasswordHash))
        {
            return ChangePasswordWithVerificationResult.InvalidPassword;
        }

        user.PasswordHash = PasswordUtils.HashPassword(newPassword);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ChangePasswordWithVerificationResult.Success;
    }
}
