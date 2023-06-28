using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.DTOs;

public static class UserMapper
{
    public static UserDto ToUserDto(this UserEntity user, UserRelationEntity? userRelation)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            AvatarUrl = user.ProfileAvatar?.PublicUrl,
            BannerUrl = user.ProfileBanner?.PublicUrl,
            Status = user.Status,
            StatusText = user.StatusText,
            RelationType = userRelation?.RelationType ?? UserRelationType.None,
            NickName = userRelation?.NickName,
            Notes = userRelation?.Notes,
            CreatedAt = user.CreatedAt,
            LastSeenAt = user.LastOnline,
            FriendedAt = userRelation?.CreatedAt
        };
    }

    public static UserDto ToMinimalUserDto(this UserEntity user, UserRelationEntity? userRelation)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            AvatarUrl = null,
            BannerUrl = null,
            Status = UserStatus.Offline,
            StatusText = String.Empty,
            RelationType = userRelation?.RelationType ?? UserRelationType.None,
            NickName = userRelation?.NickName,
            Notes = userRelation?.Notes,
            CreatedAt = DateTime.MinValue,
            LastSeenAt = DateTime.MinValue
        };
    }

    public static AccountDto ToAccountDto(this UserEntity user)
    {
        return new AccountDto
        {
            Id = user.Id,
            Username = user.Name,
            ObscuredEmail = Transformers.ObscureEmail(user.Email),
            EmailVerified = user.EmailVerified,
            AcceptedPrivacyPolicyVersion = user.AcceptedPrivacyPolicyVersion,
            AcceptedTermsOfServiceVersion = user.AcceptedTermsOfServiceVersion,
            AvatarUrl = user.ProfileAvatar?.PublicUrl,
            BannerUrl = user.ProfileBanner?.PublicUrl,
            Status = user.Status,
            StatusText = user.StatusText,
            FriendUserIds = user.RelationsOutgoing.Select(fs => fs.TargetUserId),
            SSOConnections = user.SSOConnections.Select(oc => oc.ProviderName),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastOnline = user.LastOnline,
        };
    }
}
