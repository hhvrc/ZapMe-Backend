using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.User.FriendRequest.Models;

/// <summary>
/// List of incoming and outgoing friendrequests
/// </summary>
public struct FriendRequestList
{
    /// <summary>
    /// UserId's of users that has sent friend requests to this account
    /// </summary>
    [JsonPropertyName("incoming")]
    public Guid[] Incoming { get; set; }

    /// <summary>
    /// UserId's of users that this account has sent friend requests to
    /// </summary>
    [JsonPropertyName("outgoing")]
    public Guid[] Outgoing { get; set; }
}