namespace ZapMe.Controllers.Api.V1.Account.Models;

/// <summary>
/// Request sent to server to update account email address
/// </summary>
public readonly struct UpdateEmailOk
{
    /// <summary>
    /// Example: "Please check your email to verify your new address."
    /// </summary>
    public required string Message { get; init; }
}