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
            AvatarUrl = user.ProfileAvatar?.PublicUrl,
            BannerUrl = user.ProfileBanner?.PublicUrl,
            Status = user.Status,
            StatusText = user.StatusText,
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
            AvatarUrl = null,
            BannerUrl = null,
            Status = UserStatus.Offline,
            StatusText = String.Empty,
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
            AvatarUrl = user.ProfileAvatar?.PublicUrl,
            BannerUrl = user.ProfileBanner?.PublicUrl,
            Status = user.Status,
            StatusText = user.StatusText,
            Friends = user.Relations?.Select(fs => fs.TargetUserId) ?? Enumerable.Empty<Guid>(),
            SSOConnections = user.SSOConnections?.Select(oc => oc.ProviderName) ?? Enumerable.Empty<string>(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastOnline = user.LastOnline,
        };
    }
}
