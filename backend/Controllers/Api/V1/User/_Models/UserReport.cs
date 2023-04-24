using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.User.Models;

/// <summary>
/// Message sent to server to report a user
/// </summary>
public readonly struct UserReport
{
    [JsonPropertyName("user_id")]
    public Guid UserId { get; init; }

    /// <summary/>
    [JsonPropertyName("title")]
    public string Title { get; init; }

    /// <summary/>
    [JsonPropertyName("explenation")]
    public string Explenation { get; init; }
}