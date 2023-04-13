using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account email address
/// </summary>
public struct UpdateEmail
{
    /// <summary/>
    [EmailAddress]
    [JsonPropertyName("new_email")]
    public string NewEmail { get; set; }

    /// <summary/>
    [Password(false)]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}