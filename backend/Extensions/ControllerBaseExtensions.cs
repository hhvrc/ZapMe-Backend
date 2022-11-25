using ZapMe.Controllers;
using ZapMe.Controllers.Api.V1.Models;

namespace Microsoft.AspNetCore.Mvc;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Tries to get the remote IP address of the client based on multiple headers with a fallback to <see cref="HttpContext"/>'s Remote IP.
    /// </summary>
    /// <param name="controllerBase"></param>
    /// <returns></returns>
    public static string GetRemoteIP(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetRemoteIP();

    /// <summary>
    /// Uses Cloudflare's cf-ipcontry header to determine the country code of the request.
    /// </summary>
    /// <param name="controllerBase"></param>
    /// <returns>A 2 character country code, or null if the header is not present.</returns>
    public static string GetCloudflareIPCountry(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetCloudflareIPCountry();

    /// <summary>
    /// Returns the User-Agent header of the request.
    /// </summary>
    /// <param name="controllerBase"></param>
    /// <returns></returns>
    public static string GetRemoteUserAgent(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetRemoteUserAgent();

    private static Dictionary<string, string[]>? ToDict(IEnumerable<(string Key, string[] Value)> items) => items.ToDictionary(static x => x.Key, static x => x.Value);
    private static Dictionary<string, string[]>? ToDict(IEnumerable<(string Key, string Value)> items) => items.ToDictionary(static x => x.Key, static x => new string[] { x.Value });
    private static Dictionary<string, string[]>? ToDict(IEnumerable<string> keys, string value) => keys.ToDictionary(static x => x, x => new string[] { value });

    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, string? suggestion = null) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, suggestion, null, null);
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, UserNotification.SeverityLevel notificationSeverity, string notificationTitle, string notificationDetail) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, null, null, new UserNotification(notificationSeverity, notificationTitle, notificationDetail));
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, string? suggestion, UserNotification.SeverityLevel notificationSeverity, string notificationTitle, string notificationDetail) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, suggestion, null, new UserNotification(notificationSeverity, notificationTitle, notificationDetail));
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, string? suggestion = null, params (string field, string error)[] fields) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, suggestion, ToDict(fields), null);
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, string? suggestion = null, params (string field, string[] errors)[] fields) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, suggestion, ToDict(fields), null);
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, UserNotification.SeverityLevel notificationSeverity, string notificationTitle, string notificationDetail, params (string field, string error)[] fields) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, null, ToDict(fields), new UserNotification(notificationSeverity, notificationTitle, notificationDetail));
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, UserNotification.SeverityLevel notificationSeverity, string notificationTitle, string notificationDetail, params (string field, string[] errors)[] fields) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, null, ToDict(fields), new UserNotification(notificationSeverity, notificationTitle, notificationDetail));
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, string? suggestion, UserNotification.SeverityLevel notificationSeverity, string notificationTitle, string notificationDetail, params (string field, string error)[] fields) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, suggestion, ToDict(fields), new UserNotification(notificationSeverity, notificationTitle, notificationDetail));
    public static IActionResult Error(this ControllerBase controllerBase, int httpCode, string title, string detail, string? suggestion, UserNotification.SeverityLevel notificationSeverity, string notificationTitle, string notificationDetail, params (string field, string[] errors)[] fields) =>
        ErrorResponseFactory.CreateActionResult_Error(controllerBase.HttpContext, httpCode, title, detail, suggestion, ToDict(fields), new UserNotification(notificationSeverity, notificationTitle, notificationDetail));

    public static IActionResult Error_InvalidModelState(this ControllerBase controllerBase, params (string field, string error)[] fields) =>
        ErrorResponseFactory.CreateActionResult_InvalidModelState(controllerBase.HttpContext, ToDict(fields));
    public static IActionResult Error_InvalidModelState(this ControllerBase controllerBase, params (string field, string[] errors)[] fields) =>
        ErrorResponseFactory.CreateActionResult_InvalidModelState(controllerBase.HttpContext, ToDict(fields));

    public static IActionResult Error_AnonymousOnly(this ControllerBase controllerBase) =>
        ErrorResponseFactory.CreateActionResult_AnonymousOnly(controllerBase.HttpContext);
    public static IActionResult Error_InvalidCredentials(this ControllerBase controllerBase, string message, string notificationMessage, params string[] fields) =>
        ErrorResponseFactory.CreateActionResult_InvalidCredentials(controllerBase.HttpContext, ToDict(fields, message), notificationMessage);
    public static IActionResult Error_InvalidPassword(this ControllerBase controllerBase, params string[] fields) =>
        ErrorResponseFactory.CreateActionResult_InvalidPassword(controllerBase.HttpContext, ToDict(fields, "Invalid password"));
    public static IActionResult Error_TooManyRequests(this ControllerBase controllerBase) =>
        ErrorResponseFactory.CreateActionResult_TooManyRequests(controllerBase.HttpContext);
    public static IActionResult Error_InternalServerError(this ControllerBase controllerBase) =>
        ErrorResponseFactory.CreateActionResult_InternalServerError(controllerBase.HttpContext);
}