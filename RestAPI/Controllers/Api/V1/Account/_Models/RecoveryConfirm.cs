using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to commit a password reset
/// </summary>
public readonly struct RecoveryConfirm
{
    /// <summary>
    /// The new password to set
    /// </summary>
    [PasswordOAPI(true)]
    public string NewPassword { get; init; }

    /// <summary>
    /// The token sent to the user's email address
    /// </summary>
    public string Token { get; init; }
}