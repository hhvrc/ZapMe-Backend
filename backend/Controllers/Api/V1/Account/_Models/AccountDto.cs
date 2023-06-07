using ZapMe.Attributes;
using ZapMe.Data.Models;
using ZapMe.Enums;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Account object, this can only retrieved for the user you are logged in as
/// </summary>
public readonly struct AccountDto
{
    public AccountDto(UserEntity user)
    {
        string? email = user.Email;
        bool emailVerified = true;
        if (String.IsNullOrEmpty(email))
        {
            emailVerified = false;
            email = user.EmailVerificationRequest?.NewEmail;
        }

        Id = user.Id;
        Username = user.Name;
        ObscuredEmail = Transformers.ObscureEmail(email!);
        EmailVerified = emailVerified;
        AcceptedPrivacyPolicyVersion = user.AcceptedPrivacyPolicyVersion;
        AcceptedTermsOfServiceVersion = user.AcceptedTermsOfServiceVersion;
        ProfilePictureUrl = user.ProfilePicture?.PublicUrl!;
        Status = user.OnlineStatus;
        StatusText = user.OnlineStatusText;
        Friends = user.Relations?.Select(fs => fs.TargetUserId).ToArray() ?? Array.Empty<Guid>();
        OauthConnections = user.OauthConnections?.Select(oc => oc.ProviderName).ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [Username(false)]
    public string Username { get; init; }

    /// <summary>
    /// Obfuscated email of your account
    /// </summary>
    [EmailAddress]
    public string ObscuredEmail { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public bool EmailVerified { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public uint AcceptedPrivacyPolicyVersion { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public uint AcceptedTermsOfServiceVersion { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Uri ProfilePictureUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public UserStatus Status { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string StatusText { get; init; }

    /// <summary>
    /// Id of friends this account has
    /// </summary>
    public Guid[] Friends { get; init; }

    /// <summary>
    /// OAuth2 providers this account is connected to
    /// </summary>
    public string[] OauthConnections { get; init; }

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