using ZapMe.Enums;

namespace ZapMe.DTOs;

/// <summary>
/// Account object, this can only retrieved for the user you are logged in as
/// </summary>
public readonly struct AccountDto
{
    public required Guid Id { get; init; }

    public required string Username { get; init; }

    /// <summary>
    /// Obfuscated email of your account
    /// </summary>
    public required string ObscuredEmail { get; init; }

    public required bool EmailVerified { get; init; }

    public required uint AcceptedPrivacyPolicyVersion { get; init; }

    public required uint AcceptedTermsOfServiceVersion { get; init; }

    public required Uri? AvatarUrl { get; init; }

    public required Uri? BannerUrl { get; init; }

    public required UserStatus Status { get; init; }

    public required string StatusText { get; init; }

    /// <summary>
    /// Ids of users this account has friended
    /// </summary>
    public required IEnumerable<Guid> FriendUserIds { get; init; }

    /// <summary>
    /// SSO providers this account is connected to
    /// </summary>
    public required IEnumerable<string> SSOConnections { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was updated
    /// </summary>
    public required DateTime UpdatedAt { get; init; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    public required DateTime LastOnline { get; init; }
}