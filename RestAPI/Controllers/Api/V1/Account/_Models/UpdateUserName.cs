using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account username
/// </summary>
public readonly struct UpdateUserName
{
    /// <summary/>
    [UsernameOAPI(true)]
    public string NewUsername { get; init; }

    /// <summary/>
    [PasswordOAPI(false)]
    public string Password { get; init; }
}