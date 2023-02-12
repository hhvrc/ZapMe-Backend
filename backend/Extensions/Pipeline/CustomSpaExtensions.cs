using ZapMe.Constants;

namespace Microsoft.AspNetCore.Builder;

public static class CustomSpaExtensions
{
    public static Task ProductionMiddleware(HttpContext context, Func<Task> next)
    {
        // Returmn 404 if the request is not for a SPA path
        if (context.Request.Path.StartsWithSegments(FrontendConstants.NonFrontendPathSegments))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

        // The request is for the SPA, redirect to the SPA index
        context.Request.Path = "/index.html";

        return next();
    }

    /// <summary>
    /// Adds the SPA middleware to the pipeline.
    /// This must be the last middleware in the pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public static void UseSpa(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../frontend";
                spa.UseProxyToSpaDevelopmentServer(FrontendConstants.FrontendDevUrl);
            });
        }
        else
        {
            app.Use(ProductionMiddleware);
            app.UseStaticFiles();
        }
    }
}