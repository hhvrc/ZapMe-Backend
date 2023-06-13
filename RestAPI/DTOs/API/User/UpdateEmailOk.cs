namespace ZapMe.DTOs.API.User;

/// <summary>
/// Request sent to server to update account email address
/// </summary>
public readonly struct UpdateEmailOk
{
    /// <summary>
    /// Example: "Please check your email to verify your new address."
    /// </summary>
    public string Message { get; init; }
}