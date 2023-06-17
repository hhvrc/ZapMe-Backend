using ZapMe.Attributes;
using ZapMe.Enums;

namespace ZapMe.DTOs;

/// <summary>
/// Account object, this can only retrieved for the user you are logged in as
/// </summary>
public readonly struct AccountDto
{
    public Guid Id { get; init; }

    [Username(false)]
    public string Username { get; init; }

    /// <summary>
    /// Obfuscated email of your account
    /// </summary>
    public string ObscuredEmail { get; init; }

    public bool EmailVerified { get; init; }

    public uint AcceptedPrivacyPolicyVersion { get; init; }

    public uint AcceptedTermsOfServiceVersion { get; init; }

    public Uri? AvatarUrl { get; init; }

    public Uri? BannerUrl { get; init; }

    public UserStatus Status { get; init; }

    public string StatusText { get; init; }

    /// <summary>
    /// Id of friends this account has
    /// </summary>
    public IEnumerable<FriendDto> Friends { get; init; }

    /// <summary>
    /// SSO providers this account is connected to
    /// </summary>
    public IEnumerable<string> SSOConnections { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was updated
    /// </summary>
    public DateTime UpdatedAt { get; init; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    public DateTime LastOnline { get; init; }
}