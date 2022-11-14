using System.Text.Json.Serialization;
using ZapMe.Attributes;
using ZapMe.Logic;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Account object, this can only retrieved for the user you are logged in as
/// </summary>
public struct AccountDto
{
    public AccountDto(Data.Models.UserEntity user)
    {
        Id = user.Id;
        UserName = user.UserName;
        ObscuredEmail = Transformers.ObscureEmail(user.Email);
        EmailVerified = user.EmailVerified;
        ConnectedAccounts = user.OauthConnections?.Select(oc => oc.ProviderName).ToArray() ?? Array.Empty<string>();
        Friends = user.Relations?.Select(fs => fs.TargetUserId).ToArray() ?? Array.Empty<Guid>();
        CreatedAt = user.CreatedAt;
    }

    /// <summary/>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary/>
    [Username(false)]
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    /// <summary>
    /// Obfuscated email of your account
    /// </summary>
    [EmailAddress(false)]
    [JsonPropertyName("email")]
    public string ObscuredEmail { get; set; }

    /// <summary>
    /// True if email address has been verified
    /// </summary>
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// OAuth2 providers this account is connected to
    /// </summary>
    [JsonPropertyName("connected_accounts")]
    public string[] ConnectedAccounts { get; set; }

    /// <summary>
    /// Id of friends this account has
    /// </summary>
    [JsonPropertyName("friends")]
    public Guid[] Friends { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}