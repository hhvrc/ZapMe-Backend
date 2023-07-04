using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.DTOs;

public static class UserMapper
{
    public static UserDto MapToDto(UserEntity user, UserRelationDto userRelation)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            AvatarUrl = user.ProfileAvatar?.PublicUrl,
            BannerUrl = user.ProfileBanner?.PublicUrl,
            Status = user.Status,
            StatusText = user.StatusText,
            FriendStatus = userRelation.FriendStatus,
            IsFavorite = userRelation.IsFavorite,
            IsMuted = userRelation.IsMuted,
            NickName = userRelation.NickName,
            Notes = userRelation.Notes,
            CreatedAt = user.CreatedAt,
            LastOnline = user.LastOnline,
            FriendedAt = userRelation.FriendedAt,
        };
    }

    public static UserDto MapToMinimalDto(UserEntity user, UserRelationDto userRelation)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            AvatarUrl = null,
            BannerUrl = null,
            Status = UserStatus.Offline,
            StatusText = String.Empty,
            FriendStatus = userRelation.FriendStatus,
            IsFavorite = userRelation.IsFavorite,
            IsMuted = userRelation.IsMuted,
            NickName = userRelation.NickName,
            Notes = userRelation.Notes,
            CreatedAt = DateTime.MinValue,
            LastOnline = DateTime.MinValue,
            FriendedAt = userRelation.FriendedAt,
        };
    }

    public static AccountDto MapToAccountDto(UserEntity user)
    {
        return new AccountDto
        {
            Id = user.Id,
            Username = user.Name,
            Email = user.Email,
            EmailVerified = user.EmailVerified,
            AcceptedPrivacyPolicyVersion = user.AcceptedPrivacyPolicyVersion,
            AcceptedTermsOfServiceVersion = user.AcceptedTermsOfServiceVersion,
            AvatarUrl = user.ProfileAvatar?.PublicUrl,
            BannerUrl = user.ProfileBanner?.PublicUrl,
            Status = user.Status,
            StatusText = user.StatusText,
            FriendUserIds = user.RelationsOutgoing.Where(r => r.FriendStatus == UserPartialFriendStatus.Accepted).Select(r => r.ToUserId),
            SSOConnections = user.SSOConnections.Select(oc => oc.ProviderName),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastOnline = user.LastOnline,
        };
    }
}
