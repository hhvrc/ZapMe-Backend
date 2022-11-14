using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Models;

/// <summary>
/// Details about the error
/// </summary>
public struct ErrorDetails
{
    public ErrorDetails(string title, string detail, string traceId, string? suggestion = null, Dictionary<string, string[]>? fields = null, UserNotification? notification = null)
    {
        Title = title;
        Detail = detail;
        TraceId = traceId;
        Suggestion = suggestion;
        Fields = fields;
        Notification = notification;
    }

    /// <summary>
    /// Title for developer to understand what went wrong (not user friendly)
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// More detailed description of what this error is about (not user friendly)
    /// </summary>
    [JsonPropertyName("detail")]
    public string Detail { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; }

    /// <summary>
    /// Suggestion to developer on how they might be able to midegate this error
    /// </summary>
    [JsonPropertyName("suggestion")]
    public string? Suggestion { get; set; }

    /// <summary>
    /// Errors for specific fields in the request
    /// </summary>
    [JsonPropertyName("fields")]
    public Dictionary<string, string[]>? Fields { get; set; }

    /// <summary>
    /// This is a user friendly error message, might help the user understand what went wrong and how to fix it
    /// Completely optional, might be null
    /// </summary>
    [JsonPropertyName("notification")]
    public UserNotification? Notification { get; set; }
}
