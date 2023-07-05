using ZapMe.Enums;

namespace ZapMe.DTOs;

public readonly struct UserRelationDto
{
    public required UserRelationType Type { get; init; }

    public required bool IsFavorite { get; init; }

    public required bool IsMuted { get; init; }

    public required string? NickName { get; init; }

    public required string? Notes { get; init; }

    public required DateTime? FriendedAt { get; init; }
}