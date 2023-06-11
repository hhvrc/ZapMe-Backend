using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1.Models;

/// <summary>
/// Details about the error
/// </summary>
public readonly struct ErrorDetails
{
    public ErrorDetails(int httpStatusCode, string code, string detail, string? suggestion = null, Dictionary<string, string[]>? fields = null, UserNotification? notification = null)
    {
        HttpStatusCode = httpStatusCode;
        Code = code;
        Detail = detail;
        Suggestion = suggestion;
        Fields = fields;
        Notification = notification;
    }

    /// <summary>
    /// HTTP status code
    /// </summary>
    [JsonIgnore]
    public int HttpStatusCode { get; init; }

    /// <summary>
    /// Error code, this is a short string that can be used to identify the error (meant for developers)
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Detailed description of what this error is about (meant for developers)
    /// </summary>
    public string Detail { get; init; }

    /// <summary>
    /// Suggestion on how to midegate this error (meant for developers)
    /// </summary>
    public string? Suggestion { get; init; }

    /// <summary>
    /// Errors for specific fields in the request
    /// </summary>
    public Dictionary<string, string[]>? Fields { get; init; }

    /// <summary>
    /// This is a user friendly error message, might help the user understand what went wrong and how to fix it
    /// Completely optional, might be null
    /// </summary>
    public UserNotification? Notification { get; init; }

    public ObjectResult ToActionResult() => new ObjectResult(this) { StatusCode = HttpStatusCode, ContentTypes = { Application.Json } };
    public static implicit operator ObjectResult(ErrorDetails errorDetails) => errorDetails.ToActionResult();

    public Task Write(HttpResponse response, JsonSerializerOptions? options = null)
    {
        response.StatusCode = HttpStatusCode;
        response.ContentType = Application.Json;
        return response.WriteAsJsonAsync(this, options);
    }
}
