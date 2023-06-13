namespace ZapMe.DTOs.API.User;

/// <summary>
/// List of incoming and outgoing friendrequests
/// </summary>
public readonly struct FriendRequestList
{
    /// <summary>
    /// UserId's of users that has sent friend requests to this user
    /// </summary>
    public IEnumerable<Guid> Incoming { get; init; }

    /// <summary>
    /// UserId's of users that this user has sent friend requests to
    /// </summary>
    public IEnumerable<Guid> Outgoing { get; init; }
}