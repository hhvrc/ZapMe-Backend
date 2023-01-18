using System.Text.Json.Serialization;

namespace ZapMe.Authentication.Models;

/// <summary>
/// 
/// </summary>
public class SignInOk
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; set; }

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
