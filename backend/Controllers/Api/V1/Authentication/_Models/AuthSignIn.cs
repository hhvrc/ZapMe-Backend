using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Authentication.Models;

/// <summary>
/// Message sent to server to authenticate user using username and password
/// </summary>
public struct AuthSignIn
{
    /// <summary>
    /// Username or email address
    /// </summary>
    [Username(false)]
    [JsonPropertyName("username")]
    public string Username { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [Username(true)]
    [JsonPropertyName("password")]
    public string Password { get; set; }

    /// <summary>
    /// Session name to remember this sign in by, e.g. "My home computer"
    /// This is for the users to be able to see which devices they have logged in their user settings
    /// </summary>
    [StringLength(32, MinimumLength = 1)]
    [JsonPropertyName("sessionName")]
    public string SessionName { get; set; }

    /// <summary>
    /// Make this login persist for a longer period of time
    /// </summary>
    [JsonPropertyName("rememberMe")]
    public bool RememberMe { get; set; }
}