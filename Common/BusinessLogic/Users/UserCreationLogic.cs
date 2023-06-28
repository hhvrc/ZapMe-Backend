using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.BusinessLogic.Users;

public static class UserCreationLogic
{
    /// <summary>
    /// Creates a new user account that is verified and will never need to accept the privacy policy or terms of service.
    /// </summary>
    public static async Task<UserEntity> CreateVerifiedSystemUser(DatabaseContext dbContext, string username, string email, string password, CancellationToken cancellationToken)
    {
        UserEntity user = new UserEntity
        {
            Name = username,
            Email = email,
            EmailVerified = true,
            PasswordHash = PasswordUtils.HashPassword(password),
            AcceptedPrivacyPolicyVersion = UInt32.MaxValue,
            AcceptedTermsOfServiceVersion = UInt32.MaxValue,
            ProfileAvatarId = null,
            ProfileBannerId = null,
            Status = UserStatus.Online,
            StatusText = String.Empty
        };

        // Create account
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}
