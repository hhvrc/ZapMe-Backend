using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to request a password reset token
/// </summary>
public readonly struct RecoveryRequest
{
    /// <summary>
    /// Email of your account you want to recover
    /// </summary>
    [EmailAddress]
    public string Email { get; init; }

    /// <summary>
    /// Response from cloudflare turnstile request
    /// </summary>
    public string TurnstileResponse { get; init; }
}