using System.Text.Json.Serialization;
using ZapMe.Attributes;
using ZapMe.Controllers.Api.V1.User.Models;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Account object, this can only retrieved for the user you are logged in as
/// </summary>
public sealed class AccountDto : UserDto
{
    public AccountDto(Data.Models.UserEntity user)
        : base(user)
    {
        ObscuredEmail = Transformers.ObscureEmail(user.Email);
        EmailVerified = user.EmailVerified;
        AcceptedTosVersion = user.AcceptedTosVersion;
        ConnectedAccounts = user.OauthConnections?.Select(oc => oc.ProviderName).ToArray() ?? Array.Empty<string>();
        Friends = user.Relations?.Select(fs => fs.TargetUserId).ToArray() ?? Array.Empty<Guid>();
    }

    /// <summary>
    /// Obfuscated email of your account
    /// </summary>
    [EmailAddress]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("email")]
    public string ObscuredEmail { get; set; }

    /// <summary>
    /// True if email address has been verified
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(10)]
    [JsonPropertyName("accepted_tos_version")]
    public int AcceptedTosVersion { get; set; }

    /// <summary>
    /// Id of friends this account has
    /// </summary>
    [JsonPropertyOrder(11)]
    [JsonPropertyName("friends")]
    public Guid[] Friends { get; set; }

    /// <summary>
    /// OAuth2 providers this account is connected to
    /// </summary>
    [JsonPropertyOrder(12)]
    [JsonPropertyName("connected_accounts")]
    public string[] ConnectedAccounts { get; set; }
}