namespace ZapMe.DTOs;

public readonly struct FriendDto
{
    public Guid FriendId { get; init; }
    public string? NickName { get; init; }
    public string? Notes { get; init; }
    public DateTime FriendedAt { get; init; }
}