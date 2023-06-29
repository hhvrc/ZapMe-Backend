using ZapMe.Attributes;

namespace ZapMe.DTOs.API.User;

/// <summary>
/// Message sent to server to create a new account
/// </summary>
public readonly struct AccountCreateRequestDto
{
    [Username(true)]
    public string Username { get; init; }

    [Password(true)]
    public string Password { get; init; }

    [EmailAddress]
    public string Email { get; init; }

    public uint AcceptedPrivacyPolicyVersion { get; init; }

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