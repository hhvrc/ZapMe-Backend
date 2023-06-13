namespace ZapMe.DTOs;

public readonly struct SessionDto
{
    public required Guid SessionToken { get; init; }

    public required DateTime IssuedAtUtc { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }
}
