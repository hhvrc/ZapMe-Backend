using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers;

public static class ErrorResponseFactory
{
    public static IActionResult CreateActionResult_Error(HttpContext context, int httpCode, string title, string detail, string? suggestion, Dictionary<string, string[]>? fields, UserNotification? notification) =>
        new ObjectResult(new ErrorDetails(title, detail, context.TraceIdentifier, suggestion, fields, notification)) { StatusCode = httpCode, ContentTypes = { Application.Json, Application.Xml } };
    public static IActionResult CreateActionResult_InvalidModelState(HttpContext context, Dictionary<string, string[]>? fields) =>
        CreateActionResult_Error(context, StatusCodes.Status400BadRequest, "Bad Request", "Request body has invalid format/fields", null, fields, null);
    public static IActionResult CreateActionResult_AnonymousOnly(HttpContext context) =>
        CreateActionResult_Error(context, StatusCodes.Status403Forbidden, "Anonymous Only", "This endpoint is only available to anonymous users", "Please sign out, or remove the Authorization header and try again", null, null);
    public static IActionResult CreateActionResult_InvalidCredentials(HttpContext context, Dictionary<string, string[]>? fields, string notifiactionMsg) =>
        CreateActionResult_Error(context, StatusCodes.Status401Unauthorized, "Unauthorized", "Invalid credentials", null, fields, new UserNotification(UserNotification.SeverityLevel.Error, "Invalid credentials", notifiactionMsg));
    public static IActionResult CreateActionResult_InvalidPassword(HttpContext context, Dictionary<string, string[]>? fields) =>
        CreateActionResult_Error(context, StatusCodes.Status401Unauthorized, "Unauthorized", "Invalid password", null, fields, new UserNotification(UserNotification.SeverityLevel.Error, "Invalid password", "The password you entered is incorrect."));
    public static IActionResult CreateActionResult_TooManyRequests(HttpContext context) =>
        CreateActionResult_Error(context, StatusCodes.Status429TooManyRequests, "Too Many Requests", "You have exceeded the rate limit", null, null, new UserNotification(UserNotification.SeverityLevel.Warning, "Rate Limit Exceeded", "You have exceeded the rate limit. Please try again later."));
    public static IActionResult CreateActionResult_InternalServerError(HttpContext context) =>
        CreateActionResult_Error(context, StatusCodes.Status500InternalServerError, "Internal Server Error", "An internal server error occurred.", null, null, new UserNotification(UserNotification.SeverityLevel.Error, "Server error", "Please try again later"));


    public static Dictionary<string, string[]> GetErrors(ModelStateDictionary modelState)
    {
        Dictionary<string, string[]> errors = new();

        foreach ((string key, ModelStateEntry value) in modelState)
        {
            ModelErrorCollection errorCollection = value.Errors;

            string[] entryErrors = new string[errorCollection.Count];

            for (int i = 0; i < errorCollection.Count; i++)
            {
                entryErrors[i] = errorCollection[i].ErrorMessage;
            }

            errors[key] = entryErrors;
        }

        return errors;
    }

    public static IActionResult CreateErrorResult(ActionContext actionContext)
    {
        HttpContext httpContext = actionContext.HttpContext;
        ModelStateDictionary modelState = actionContext.ModelState;

        if (!modelState.IsValid)
        {
            return CreateActionResult_InvalidModelState(httpContext, GetErrors(modelState));
        }

        return CreateActionResult_InternalServerError(httpContext);
    }
}
