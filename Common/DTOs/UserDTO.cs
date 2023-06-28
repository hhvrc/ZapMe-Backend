using ZapMe.Enums;

namespace ZapMe.DTOs;

public readonly struct UserDto
{
    public Guid Id { get; init; }

    public string Username { get; init; }

    public Uri? AvatarUrl { get; init; }

    public Uri? BannerUrl { get; init; }

    public UserStatus Status { get; init; }

    public string StatusText { get; init; }

    public UserRelationType RelationType { get; init; }

    public string? NickName { get; init; }

    public string? Notes { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was seen online
    /// </summary>
    public DateTime LastSeenAt { get; init; }

    /// <summary>
    /// The time this user was friended at
    /// </summary>
    public DateTime? FriendedAt { get; init; }
}