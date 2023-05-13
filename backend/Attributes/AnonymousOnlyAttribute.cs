using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ZapMe.Helpers;

namespace ZapMe.Attributes;

public class AnonymousOnlyAttribute : ActionFilterAttribute, IAllowAnonymous
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = CreateHttpError.Generic(
                StatusCodes.Status403Forbidden,
                "anon_only",
                "This endpoint is only available to anonymous users",
                "Please sign out, or remove the Authorization header and try again"
                ).ToActionResult();
        }
    }
}
