using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1.Models;

/// <summary>
/// Details about the error
/// </summary>
public readonly struct ErrorDetails
{
    public ErrorDetails(int httpStatusCode, string title, string detail, string? suggestion = null, Dictionary<string, string[]>? fields = null, UserNotification? notification = null)
    {
        HttpStatusCode = httpStatusCode;
        Title = title;
        Detail = detail;
        Suggestion = suggestion;
        Fields = fields;
        Notification = notification;
    }

    /// <summary>
    /// HTTP status code
    /// </summary>
    [JsonIgnore]
    public int HttpStatusCode { get; }

    /// <summary>
    /// Title for developer to understand what went wrong (not user friendly)
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// More detailed description of what this error is about (not user friendly)
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// Suggestion to developer on how they might be able to midegate this error
    /// </summary>
    public string? Suggestion { get; }

    /// <summary>
    /// Errors for specific fields in the request
    /// </summary>
    public Dictionary<string, string[]>? Fields { get; }

    /// <summary>
    /// This is a user friendly error message, might help the user understand what went wrong and how to fix it
    /// Completely optional, might be null
    /// </summary>
    public UserNotification? Notification { get; }

    public ObjectResult ToActionResult() => new ObjectResult(this) { StatusCode = HttpStatusCode, ContentTypes = { Application.Json } };
    public static implicit operator ObjectResult(ErrorDetails errorDetails) => errorDetails.ToActionResult();
}
