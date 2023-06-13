using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.DTOs;

public static class UserMapper
{
    public static UserDto ToUserDto(this UserEntity user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            ProfilePictureUrl = user.ProfilePicture?.PublicUrl,
            ProfileBannerUrl = user.ProfileBanner?.PublicUrl,
            Presence = user.Presence,
            StatusMessage = user.StatusMessage,
            CreatedAt = user.CreatedAt,
            LastSeenAt = user.LastOnline,
        };
    }

    // Exposes minimal information about a user
    public static UserDto ToMinimalUserDto(this UserEntity user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            ProfilePictureUrl = null,
            ProfileBannerUrl = null,
            Presence = UserPresence.Offline,
            StatusMessage = String.Empty,
            CreatedAt = DateTime.MinValue,
            LastSeenAt = DateTime.MinValue
        };
    }

    public static AccountDto ToAccountDto(this UserEntity user)
    {
        string? email = user.Email;
        bool emailVerified = true;
        if (String.IsNullOrEmpty(email))
        {
            emailVerified = false;
            email = user.EmailVerificationRequest?.NewEmail;
        }

        return new AccountDto
        {
            Id = user.Id,
            Username = user.Name,
            ObscuredEmail = Transformers.ObscureEmail(email!),
            EmailVerified = emailVerified,
            AcceptedPrivacyPolicyVersion = user.AcceptedPrivacyPolicyVersion,
            AcceptedTermsOfServiceVersion = user.AcceptedTermsOfServiceVersion,
            ProfilePictureUrl = user.ProfilePicture?.PublicUrl!,
            Status = user.Presence,
            StatusText = user.StatusMessage,
            Friends = user.Relations?.Select(fs => fs.TargetUserId).ToArray() ?? Array.Empty<Guid>(),
            SSOConnections = user.SSOConnections?.Select(oc => oc.ProviderName).ToArray() ?? Array.Empty<string>(),
        };
    }
}
