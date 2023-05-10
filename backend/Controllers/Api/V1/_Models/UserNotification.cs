namespace ZapMe.Controllers.Api.V1.Models;

/// <summary>
/// System notification to be displayed to the user, do not confuse with user notifications
/// This is a hint and clients can choose to ignore it or implement notifications to be independent of this
/// </summary>
public sealed class UserNotification
{
    public UserNotification(SeverityLevel severity, string message)
    {
        Severity = severity;
        Content = message;
    }

    public enum SeverityLevel
    {
        Info,
        Warning,
        Error
    };

    /// <summary>
    /// Severity of the notification, might be used in clients to determine how to display the notification
    /// </summary>
    public SeverityLevel Severity { get; set; }

    /// <summary>
    /// Content of the notification, might be HTML
    /// </summary>
    public string Content { get; set; }
}
