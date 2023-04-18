using System.Text.Json.Serialization;
using ZapMe.Data.Models;

namespace ZapMe.Authentication.Models;

public sealed class SessionDto
{
    public SessionDto(SessionEntity session)
    {
        Id = session.Id;
        IssuedAtUtc = session.CreatedAt;
        ExpiresAtUtc = session.ExpiresAt;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("issuedAtUtc")]
    public DateTime IssuedAtUtc { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }
}
