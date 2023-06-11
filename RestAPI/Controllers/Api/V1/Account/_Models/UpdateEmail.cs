using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account email address
/// </summary>
public readonly struct UpdateEmail
{
    /// <summary/>
    [EmailAddress]
    public string NewEmail { get; init; }

    /// <summary/>
    [PasswordOAPI(false)]
    public string Password { get; init; }
}