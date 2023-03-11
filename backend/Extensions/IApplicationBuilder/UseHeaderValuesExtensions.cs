using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Builder;

public  static class UseHeaderValuesExtensions
{
    public static IApplicationBuilder UseHeaderValue(this IApplicationBuilder app, string key, StringValues value)
    {
        return app.Use((context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[key] = value;
                return Task.CompletedTask;
            });
            return next();
        });
    }
    public static IApplicationBuilder UseHeaderValues(this IApplicationBuilder app, params (string key, StringValues value)[] headerValues)
    {
        return app.Use((context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                foreach ((string key, StringValues value) in headerValues)
                {
                    context.Response.Headers[key] = value;
                }
                
                return Task.CompletedTask;
            });
            return next();
        });
    }
}
