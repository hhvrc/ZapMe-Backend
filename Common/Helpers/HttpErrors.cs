using Microsoft.AspNetCore.Mvc;
using ZapMe.Constants;
using ZapMe.DTOs;

namespace ZapMe.Helpers;

public static class HttpErrors
{
    private static Dictionary<string, string[]>? ToDict(IEnumerable<(string Key, string[] Value)> items) => items.ToDictionary(static x => x.Key, static x => x.Value);
    private static Dictionary<string, string[]>? ToDict(IEnumerable<(string Key, string Value)> items) => items.ToDictionary(static x => x.Key, static x => new string[] { x.Value });
    private static Dictionary<string, string[]>? ToDict(IEnumerable<string> keys, string value) => keys.ToDictionary(static x => x, x => new string[] { value });

    private static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, Dictionary<string, string[]>? fields, UserNotification? notification) =>
        new ErrorDetails(httpCode, code, detail, suggestion, fields, notification);
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion = null) =>
        Generic(httpCode, code, detail, suggestion, null, null);
    public static ErrorDetails Generic(int httpCode, string code, string detail, NotificationSeverityLevel notificationSeverity, string notificationContent) =>
        Generic(httpCode, code, detail, null, null, new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, NotificationSeverityLevel notificationSeverity, string notificationContent) =>
        Generic(httpCode, code, detail, suggestion, null, new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion = null, params (string field, string error)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), null);
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion = null, params (string field, string[] errors)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), null);
    public static ErrorDetails Generic(int httpCode, string code, string detail, NotificationSeverityLevel notificationSeverity, string notificationContent, params (string field, string error)[] fields) =>
        Generic(httpCode, code, detail, null, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, NotificationSeverityLevel notificationSeverity, string notificationContent, params (string field, string[] errors)[] fields) =>
        Generic(httpCode, code, detail, null, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, NotificationSeverityLevel notificationSeverity, string notificationContent, params (string field, string error)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));
    public static ErrorDetails Generic(int httpCode, string code, string detail, string? suggestion, NotificationSeverityLevel notificationSeverity, string notificationContent, params (string field, string[] errors)[] fields) =>
        Generic(httpCode, code, detail, suggestion, ToDict(fields), new UserNotification(notificationSeverity, notificationContent));

    /// <summary>
    /// 400 Bad Request
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static ErrorDetails InvalidModelState(Dictionary<string, string[]>? fields) =>
        Generic(StatusCodes.Status400BadRequest, "invalid_model", "Request body has invalid format/fields", null, fields, null);
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

    public static ErrorDetails InvalidPassword => Generic(StatusCodes.Status401Unauthorized, "password_invalid", "Invalid password", null, null, new UserNotification(NotificationSeverityLevel.Error, "Invalid password"));
    public static IActionResult InvalidPasswordActionResult => InvalidPassword.ToActionResult();

    public static ErrorDetails UserNameOrEmailTaken => Generic(StatusCodes.Status401Unauthorized, "unauthorized", "Username or email already taken", null, null, new UserNotification(NotificationSeverityLevel.Error, "Username or email already taken"));
    public static IActionResult UserNameOrEmailTakenActionResult => UserNameOrEmailTaken.ToActionResult();

    public static ErrorDetails UnsupportedSSOProvider(string providerName) =>
        Generic(StatusCodes.Status406NotAcceptable, "sso_provider_not_supported", $"The SSO (Single Sign On) provider \"{providerName}\" is not supported", "Get the list of supported providers from the GET /api/v1/sso/providers endpoint");

    public static ErrorDetails InvalidSSOToken => Generic(StatusCodes.Status401Unauthorized, "sso_token_invalid", "The provided SSO token is invalid or expired", "To retrieve a valid token, start the authentication flow by calling the GET /api/v1/sso/{providerName} endpoint");
    public static IActionResult InvalidSSOTokenActionResult => InvalidSSOToken.ToActionResult();

    /// <summary>
    /// 429 Too Many Requests
    /// </summary>
    /// <returns></returns>
    public static ErrorDetails TooManyRequests => Generic(StatusCodes.Status429TooManyRequests, "ratelimited", "You have exceeded the rate limit", null, null, new UserNotification(NotificationSeverityLevel.Warning, "Rate limit exceeded, please try again later"));
    public static IActionResult TooManyRequestsActionResult => TooManyRequests.ToActionResult();

    /// <summary>
    /// 500 Internal Server Error
    /// </summary>
    /// <returns></returns>
    public static ErrorDetails InternalServerError => Generic(StatusCodes.Status500InternalServerError, "internal_error", "An internal server error occurred.", null, null, new UserNotification(NotificationSeverityLevel.Error, "Internal server error, please try again later"));
    public static IActionResult InternalServerErrorActionResult => InternalServerError.ToActionResult();

    public static ErrorDetails Unauthorized => Generic(StatusCodes.Status401Unauthorized, "unauthorized", "You are not authorized to perform this action", null, null, new UserNotification(NotificationSeverityLevel.Error, "You are not authorized to perform this action"));
    public static IActionResult UnauthorizedActionResult => Unauthorized.ToActionResult();

    public static ErrorDetails Forbidden => Generic(StatusCodes.Status403Forbidden, "forbidden", "You are not allowed to perform this action", null, null, new UserNotification(NotificationSeverityLevel.Error, "You are not allowed to perform this action"));
    public static IActionResult ForbiddenActionResult => Forbidden.ToActionResult();

    public static ErrorDetails UserNotFound => Generic(StatusCodes.Status404NotFound, "user_not_found", "User not found", null, null, new UserNotification(NotificationSeverityLevel.Error, "User not found"));
    public static IActionResult UserNotFoundActionResult => UserNotFound.ToActionResult();

    public static ErrorDetails DeviceNotFound => Generic(StatusCodes.Status404NotFound, "device_not_found", "Device not found", null, null, new UserNotification(NotificationSeverityLevel.Error, "Device not found"));
    public static IActionResult DeviceNotFoundActionResult => DeviceNotFound.ToActionResult();

    public static ErrorDetails UnverifiedEmail => HttpErrors.Generic(StatusCodes.Status400BadRequest, "Unverified Email", "Email has not been verified", NotificationSeverityLevel.Warning, "Please verify your email address, and then try again");
    public static IActionResult UnverifiedEmailActionResult => UnverifiedEmail.ToActionResult();

    public static ErrorDetails ReviewPrivacyPolicy => HttpErrors.Generic(StatusCodes.Status400BadRequest, "review_privpol", "User needs to accept new Privacy Policy", NotificationSeverityLevel.Error, "Please read and accept the new Privacy Policy, and then try again");
    public static IActionResult ReviewPrivacyPolicyActionResult => ReviewPrivacyPolicy.ToActionResult();

    public static ErrorDetails ReviewTermsOfService => HttpErrors.Generic(StatusCodes.Status400BadRequest, "review_tos", "User needs to accept new Terms of Service", NotificationSeverityLevel.Error, "Please read and accept the new Terms of Service, and then try again");
    public static IActionResult ReviewTermsOfServiceActionResult => ReviewTermsOfService.ToActionResult();

    public static ErrorDetails UserAgentTooLong => HttpErrors.Generic(StatusCodes.Status413RequestEntityTooLarge, "User-Agent too long", $"User-Agent header has a hard limit on {UserAgentLimits.MaxUploadLength} characters", NotificationSeverityLevel.Error, "Unexpected behaviour, please contact developers");
    public static IActionResult UserAgentTooLongActionResult => UserAgentTooLong.ToActionResult();
}
