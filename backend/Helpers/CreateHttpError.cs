using ZapMe.Controllers.Api.V1.Models;

namespace ZapMe.Helpers;

public static class CreateHttpError
{
    private static Dictionary<string, string[]>? ToDict(IEnumerable<(string Key, string[] Value)> items) => items.ToDictionary(static x => x.Key, static x => x.Value);
    private static Dictionary<string, string[]>? ToDict(IEnumerable<(string Key, string Value)> items) => items.ToDictionary(static x => x.Key, static x => new string[] { x.Value });
    private static Dictionary<string, string[]>? ToDict(IEnumerable<string> keys, string value) => keys.ToDictionary(static x => x, x => new string[] { value });

    private static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, Dictionary<string, string[]>? fields, UserNotification? notification) =>
        new ErrorDetails(httpCode, code, detail, suggestion, fields, notification);
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion = null) =>
        Generic(httpCode, code, detail, suggestion, null, null);
    public static ErrorDetails Generic(int httpCode, string code, string detail, UserNotification.SeverityLevel notificationSeverity, string notificationContent) =>
        Generic(httpCode, code, detail, null, null, new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, UserNotification.SeverityLevel notificationSeverity, string notificationContent) =>
        Generic(httpCode, code, detail, suggestion, null, new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion = null, params (string field, string error)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), null);
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion = null, params (string field, string[] errors)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), null);
    public static ErrorDetails Generic(int httpCode, string code, string detail, UserNotification.SeverityLevel notificationSeverity, string notificationContent, params (string field, string error)[] fields) =>
        Generic(httpCode, code, detail, null, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, UserNotification.SeverityLevel notificationSeverity, string notificationContent, params (string field, string[] errors)[] fields) =>
        Generic(httpCode, code, detail, null, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, UserNotification.SeverityLevel notificationSeverity, string notificationContent, params (string field, string error)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, UserNotification.SeverityLevel notificationSeverity, string notificationContent, params (string field, string[] errors)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));

    /// <summary>
    /// 400 Bad Request
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static ErrorDetails InvalidModelState(Dictionary<string, string[]>? fields) =>
        Generic(StatusCodes.Status400BadRequest, "Bad Request", "Request body has invalid format/fields", null, fields, null);
    /// <summary>
    /// 400 Bad Request
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static ErrorDetails InvalidModelState(params (string field, string error)[] fields) =>
        InvalidModelState(ToDict(fields));
    /// <summary>
    /// 400 Bad Request
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static ErrorDetails InvalidModelState(params (string field, string[] errors)[] fields) =>
        InvalidModelState(ToDict(fields));
    /// <summary>
    /// 403 Forbidden
    /// </summary>
    /// <returns></returns>
    public static ErrorDetails AnonymousOnly() =>
        Generic(StatusCodes.Status403Forbidden, "Anonymous Only", "This endpoint is only available to anonymous users", "Please sign out, or remove the Authorization header and try again", null, null);
    /// <summary>
    /// 401 Unauthorized
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static ErrorDetails InvalidPassword(params string[] fields) =>
        Generic(StatusCodes.Status401Unauthorized, "Unauthorized", "Invalid password", null, ToDict(fields, "Invalid password"), new UserNotification(UserNotification.SeverityLevel.Error, "Invalid password"));
    public static ErrorDetails UserNameOrEmailTaken() =>
        Generic(StatusCodes.Status401Unauthorized, "Unauthorized", "Username or email already taken", null, null, new UserNotification(UserNotification.SeverityLevel.Error, "Username or email already taken"));
    /// <summary>
    /// 429 Too Many Requests
    /// </summary>
    /// <returns></returns>
    public static ErrorDetails TooManyRequests() =>
        Generic(StatusCodes.Status429TooManyRequests, "Too Many Requests", "You have exceeded the rate limit", null, null, new UserNotification(UserNotification.SeverityLevel.Warning, "Rate limit exceeded, please try again later"));
    /// <summary>
    /// 500 Internal Server Error
    /// </summary>
    /// <returns></returns>
    public static ErrorDetails InternalServerError() =>
        Generic(StatusCodes.Status500InternalServerError, "Internal Server Error", "An internal server error occurred.", null, null, new UserNotification(UserNotification.SeverityLevel.Error, "Internal server error, please try again later"));
}
