using ZapMe.Attributes;
using ZapMe.Enums;

namespace ZapMe.DTOs;

public readonly struct UserDto
{
    public Guid Id { get; init; }

    [Username(false)]
    public string Username { get; init; }

    public Uri? AvatarUrl { get; init; }

    public Uri? BannerUrl { get; init; }

    public UserStatus Status { get; init; }

    public string StatusText { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was seen online
    /// </summary>
    public DateTime LastSeenAt { get; init; }
}