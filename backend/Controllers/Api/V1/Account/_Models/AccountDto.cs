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
        AcceptedTosVersion = user.AcceptedTosVersion;
        ProfilePictureUrl = user.ProfilePicture?.PublicUrl!;
        Status = user.OnlineStatus;
        StatusText = user.OnlineStatusText;
        Friends = user.Relations?.Select(fs => fs.TargetUserId).ToArray() ?? Array.Empty<Guid>();
        OauthConnections = user.OauthConnections?.Select(oc => oc.ProviderName).ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// 
    /// </summary>
    [Username(false)]
    public string Username { get; }

    /// <summary>
    /// Obfuscated email of your account
    /// </summary>
    [EmailAddress]
    public string ObscuredEmail { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool EmailVerified { get; }

    /// <summary>
    /// 
    /// </summary>
    public int AcceptedTosVersion { get; }

    /// <summary>
    /// 
    /// </summary>
    public Uri ProfilePictureUrl { get; }

    /// <summary>
    /// 
    /// </summary>
    public UserStatus Status { get; }

    /// <summary>
    /// 
    /// </summary>
    public string StatusText { get; }

    /// <summary>
    /// Id of friends this account has
    /// </summary>
    public Guid[] Friends { get; }

    /// <summary>
    /// OAuth2 providers this account is connected to
    /// </summary>
    public string[] OauthConnections { get; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Last time this user was updated
    /// </summary>
    public DateTime UpdatedAt { get; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    public DateTime LastOnline { get; }
}