using System.ComponentModel.DataAnnotations;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Authentication.Models;

/// <summary>
/// Message sent to server to authenticate user using username and password
/// </summary>
public readonly struct AuthSignIn
{
    /// <summary>
    /// Username or email address
    /// </summary>
    [Username(false)]
    public string UsernameOrEmail { get; init; }

    /// <summary>
    /// Password
    /// </summary>
    [Password(false)]
    public string Password { get; init; }

    /// <summary>
    /// Session name to remember this sign in by, e.g. "My home computer"
    /// This is for the users to be able to see which devices they have logged in their user settings
    /// </summary>
    [StringLength(32, MinimumLength = 1)]
    public string? SessionName { get; init; }

    /// <summary>
    /// Make this login persist for a longer period of time
    /// </summary>
    public bool RememberMe { get; init; }
}