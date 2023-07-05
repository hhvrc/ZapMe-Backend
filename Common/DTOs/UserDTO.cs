using ZapMe.DTOs.Interfaces;
using ZapMe.Enums;

namespace ZapMe.DTOs;

public readonly struct UserDto : IUserDto
{
    public required Guid Id { get; init; }

    public required string Username { get; init; }

    public required Uri? AvatarUrl { get; init; }

    public required Uri? BannerUrl { get; init; }

    public required UserStatus Status { get; init; }

    public required string StatusText { get; init; }

    public required UserRelationType Relation { get; init; }

    public required bool IsFavorite { get; init; }

    public required bool IsMuted { get; init; }

    public required string? NickName { get; init; }

    public required string? Notes { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was seen online
    /// </summary>
    public required DateTime LastOnline { get; init; }

    /// <summary>
    /// The time this user was friended at
    /// </summary>
    public required DateTime? FriendedAt { get; init; }
}