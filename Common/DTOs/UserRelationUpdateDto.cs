namespace ZapMe.DTOs;

public readonly record struct UserRelationUpdateDto(bool? IsFavorite, bool? IsMuted, string? NickName, string? Notes);