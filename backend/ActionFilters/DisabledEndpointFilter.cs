using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ZapMe.ActionFilters;

public sealed class DisabledEndpointFilter : ActionFilterAttribute
{
    public string EndpointName { get; set; }

    public DisabledEndpointFilter(string endpointName)
    {
        EndpointName = endpointName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        bool isDisabled = false;

        // TODO: check if endpoint is disabled

        if (isDisabled)
        {
            context.Result = new ContentResult
            {
                Content = "This endpoint is disabled",
                StatusCode = 503
            };
        }
    }
}
