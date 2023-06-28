using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Builder;

public static class UseHeaderValuesExtensions
{
    /// <summary>
    /// Use a given header value
    /// </summary>
    /// <param name="app"></param>
    /// <param name="key">The header key.</param>
    /// <param name="value">Value to set the header to.</param>
    /// <param name="overrideExisting">If true, will override existing headers with the same key.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseHeaderValue(this IApplicationBuilder app, string key, StringValues value, bool overrideExisting = true)
    {
        if (overrideExisting)
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
        else
        {
            return app.Use((context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    if (!context.Response.Headers.ContainsKey(key))
                    {
                        context.Response.Headers[key] = value;
                    }
                    return Task.CompletedTask;
                });
                return next();
            });
        }
    }

    /// <summary>
    /// Use a list of given header values, these WILL override existing headers.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="headerValues"></param>
    /// <returns></returns>
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
