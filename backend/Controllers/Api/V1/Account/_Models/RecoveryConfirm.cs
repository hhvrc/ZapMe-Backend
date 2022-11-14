using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to commit a password reset
/// </summary>
public struct RecoveryConfirm
{
    /// <summary>
    /// The new password to set
    /// </summary>
    [Password(true)]
    [JsonPropertyName("new_password")]
    public string NewPassword { get; set; }

    /// <summary>
    /// The token sent to the user's email address
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; }
}