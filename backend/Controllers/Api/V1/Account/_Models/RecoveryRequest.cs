using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to request a password reset token
/// </summary>
public struct RecoveryRequest
{
    /// <summary>
    /// Email of your account you want to recover
    /// </summary>
    [EmailAddress(true)]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    /// <summary>
    /// Response from google recaptcha request
    /// </summary>
    [JsonPropertyName("recaptcha_response")]
    public string ReCaptchaResponse { get; set; }
}