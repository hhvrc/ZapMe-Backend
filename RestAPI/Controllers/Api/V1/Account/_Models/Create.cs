using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Message sent to server to create a new account
/// </summary>
public readonly struct CreateAccount
{
    /// <summary/>
    [UsernameOAPI(true)]
    public string Username { get; init; }

    /// <summary/>
    [PasswordOAPI(true)]
    public string Password { get; init; }

    /// <summary/>
    [EmailAddress]
    public string Email { get; init; }

    /// <summary/>
    public uint AcceptedPrivacyPolicyVersion { get; init; }

    /// <summary/>
    public uint AcceptedTermsOfServiceVersion { get; init; }

    /// <summary>
    /// Response from cloudflare turnstile request
    /// </summary>
    public string TurnstileResponse { get; init; }

    /// <summary>
    /// SSO token from OAuth flow, this is optional and only used when creating an account from the OAuth flow
    /// </summary>
    public string? SSOToken { get; init; }
}