using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ZapMe.Helpers;

namespace ZapMe.Attributes;

/// <summary>
/// An attribute that restricts access to an action method to anonymous users only.
/// If an authenticated user attempts to access the action method, a 403 Forbidden response
/// with a custom error message is returned. This attribute can be applied to an action method
/// in an ASP.NET Core application.
/// </summary>
public class AnonymousOnlyAttribute : ActionFilterAttribute, IAllowAnonymous
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = HttpErrors.Generic(
                StatusCodes.Status403Forbidden,
                "anon_only",
                "This endpoint is only available to anonymous users",
                "Please sign out, or remove the Authorization header and try again"
                ).ToActionResult();
        }
    }
}
