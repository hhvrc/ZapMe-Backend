using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Message sent to server to create a new account
/// </summary>
public struct Create
{
    /// <summary/>
    [Username(true)]
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    /// <summary/>
    [Password(true)]
    [JsonPropertyName("password")]
    public string Password { get; set; }

    /// <summary/>
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    /// <summary/>
    [JsonPropertyName("acceptedTosVersion")]
    public int AcceptedTosVersion { get; set; }

    /// <summary>
    /// Response from cloudflare turnstile request
    /// </summary>
    [JsonPropertyName("turnstile_response")]
    public string TurnstileResponse { get; set; }
}