using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.User.Models;

/// <summary>
/// User object, this can be retrieved by anyone who is friends with the user
/// </summary>
public struct User
{
    /// <summary/>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary/>
    [Username(false)]
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    [JsonPropertyName("last_online")]
    public DateTime LastOnline { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}