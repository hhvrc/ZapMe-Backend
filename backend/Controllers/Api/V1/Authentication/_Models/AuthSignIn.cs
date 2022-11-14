using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Authentication.Models;

/// <summary>
/// Message sent to server to authenticate user using username and password
/// </summary>
public struct AuthSignIn
{
    /// <summary/>
    [Username(false)]
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    /// <summary/>
    [Username(true)]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}