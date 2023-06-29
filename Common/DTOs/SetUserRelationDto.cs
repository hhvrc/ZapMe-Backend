namespace ZapMe.DTOs;

public readonly record struct SetUserRelationDto(bool? IsFavorite, bool? IsMuted, string? NickName, string? Notes);