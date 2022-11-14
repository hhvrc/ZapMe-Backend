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
    [EmailAddress(true)]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    /// <summary/>
    [JsonPropertyName("acceptedTosVersion")]
    public int AcceptedTosVersion { get; set; }

    /// <summary>
    /// Response from google recaptcha request
    /// </summary>
    [JsonPropertyName("recaptcha_response")]
    public string ReCaptchaResponse { get; set; }
}