using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account username
/// </summary>
public readonly struct UpdateUserName
{
    /// <summary/>
    [Username(true)]
    [JsonPropertyName("new_username")]
    public string NewUserName { get; init; }

    /// <summary/>
    [Password(false)]
    [JsonPropertyName("password")]
    public string Password { get; init; }
}