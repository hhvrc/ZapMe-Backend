using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Models;

/// <summary>
/// System notification to be displayed to the user, do not confuse with user notifications
/// This is a hint and clients can choose to ignore it or implement notifications to be independent of this
/// </summary>
public sealed class UserNotification
{
    public UserNotification(SeverityLevel severity, string title, string? detail = null)
    {
        Severity = severity;
        Title = title;
        Message = detail;
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
    /// Message title to display to the user
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// User friendly message about this notification
    /// </summary>
    public string? Message { get; set; }
}
