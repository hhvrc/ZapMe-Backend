using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Message sent to server to create a new account
/// </summary>
public readonly struct CreateAccount
{
    /// <summary/>
    [Username(true)]
    [JsonPropertyName("username")]
    public string UserName { get; init; }

    /// <summary/>
    [Password(true)]
    [JsonPropertyName("password")]
    public string Password { get; init; }

    /// <summary/>
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; init; }

    /// <summary/>
    [JsonPropertyName("acceptedTosVersion")]
    public int AcceptedTosVersion { get; init; }

    /// <summary>
    /// Response from cloudflare turnstile request
    /// </summary>
    [JsonPropertyName("turnstile_response")]
    public string TurnstileResponse { get; init; }
}