using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.User.FriendRequest.Models;

/// <summary>
/// List of incoming and outgoing friendrequests
/// </summary>
public readonly struct FriendRequestList
{
    /// <summary>
    /// UserId's of users that has sent friend requests to this user
    /// </summary>
    [JsonPropertyName("incoming")]
    public IEnumerable<Guid> Incoming { get; init; }

    /// <summary>
    /// UserId's of users that this user has sent friend requests to
    /// </summary>
    [JsonPropertyName("outgoing")]
    public IEnumerable<Guid> Outgoing { get; init; }
}