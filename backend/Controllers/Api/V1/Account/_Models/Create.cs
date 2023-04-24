using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Message sent to server to create a new account
/// </summary>
public readonly struct CreateAccount
{
    /// <summary/>
    [Username(true)]
    public string Username { get; init; }

    /// <summary/>
    [Password(true)]
    public string Password { get; init; }

    /// <summary/>
    [EmailAddress]
    public string Email { get; init; }

    /// <summary/>
    public int AcceptedTosVersion { get; init; }

    /// <summary>
    /// Response from cloudflare turnstile request
    /// </summary>
    public string TurnstileResponse { get; init; }
}