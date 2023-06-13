using ZapMe.Attributes;

namespace ZapMe.DTOs.API.User;

/// <summary>
/// Request sent to server to update account email address
/// </summary>
public readonly struct UpdateEmail
{
    /// <summary/>
    [EmailAddress]
    public string NewEmail { get; init; }

    /// <summary/>
    [Password(false)]
    public string Password { get; init; }
}