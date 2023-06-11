using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account password
/// </summary>
public readonly struct UpdatePassword
{
    public const string NewPassword_JsonName = "password_new";
    public const string CurrentPassword_JsonName = "password_current";

    /// <summary/>
    [PasswordOAPI(true)]
    [JsonPropertyName(NewPassword_JsonName)]
    public string NewPassword { get; init; }

    /// <summary/>
    [PasswordOAPI(false)]
    [JsonPropertyName(CurrentPassword_JsonName)]
    public string CurrentPassword { get; init; }
}