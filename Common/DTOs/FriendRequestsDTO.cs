namespace ZapMe.DTOs;

/// <summary>
/// List of incoming and outgoing friendrequests
/// </summary>
public readonly struct FriendRequestsDto
{
    /// <summary>
    /// UserId's of users that has sent friend requests to this user
    /// </summary>
    public Guid[] Incoming { get; init; }

    /// <summary>
    /// UserId's of users that this user has sent friend requests to
    /// </summary>
    public Guid[] Outgoing { get; init; }
}