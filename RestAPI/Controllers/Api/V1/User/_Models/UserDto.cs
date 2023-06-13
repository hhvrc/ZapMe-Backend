using ZapMe.Attributes;
using ZapMe.Database.Models;
using ZapMe.Enums;

namespace ZapMe.Controllers.Api.V1.User.Models;

public readonly struct UserDto
{
    public UserDto()
    {
        Id = Guid.Empty;
        Username = String.Empty;
        ProfilePictureUrl = null;
        ProfileBannerUrl = null;
        Presence = UserPresence.Offline;
        StatusMessage = String.Empty;
        CreatedAt = DateTime.MinValue;
        LastSeenAt = DateTime.MinValue;
    }
    public UserDto(UserEntity user)
    {
        Id = user.Id;
        Username = user.Name;
        ProfilePictureUrl = user.ProfilePicture?.PublicUrl;
        ProfileBannerUrl = user.ProfileBanner?.PublicUrl;
        Presence = user.Presence;
        StatusMessage = user.StatusMessage;
        CreatedAt = user.CreatedAt;
        LastSeenAt = user.LastOnline;
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [UsernameOAPI(false)]
    public string Username { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Uri? ProfilePictureUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Uri? ProfileBannerUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public UserPresence Presence { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string StatusMessage { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was seen online
    /// </summary>
    public DateTime LastSeenAt { get; init; }
}