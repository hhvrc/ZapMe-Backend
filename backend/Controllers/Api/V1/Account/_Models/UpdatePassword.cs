using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account password
/// </summary>
public struct UpdatePassword
{
    /// <summary/>
    [Password(true)]
    [JsonPropertyName("new")]
    public string NewPassword { get; set; }

    /// <summary/>
    [Password(false)]
    [JsonPropertyName("old")]
    public string Password { get; set; }
}