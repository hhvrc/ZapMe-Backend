using ZapMe.Attributes;

namespace ZapMe.DTOs.API.User;

/// <summary>
/// Request sent to server to update account username
/// </summary>
public readonly struct UpdateUserName
{
    /// <summary/>
    [Username(true)]
    public string NewUsername { get; init; }

    /// <summary/>
    [Password(false)]
    public string Password { get; init; }
}