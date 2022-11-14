using ZapMe.Controllers;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;

namespace Microsoft.AspNetCore.Mvc;

public static class ControllerBaseExtensions
{
    public static string GetRemoteIP(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetRemoteIP();
    public static SignInEntity? GetSignIn(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetSignIn();

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