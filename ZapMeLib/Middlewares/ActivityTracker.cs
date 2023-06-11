using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Data;

namespace ZapMe.Middlewares;

public sealed class ActivityTracker
{
    private readonly RequestDelegate _next;

    public ActivityTracker(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ZapMeContext dbContext)
    {
        // Add TraceIdentifier to the response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["Trace-Id"] = context.TraceIdentifier;
            return Task.CompletedTask;
        });

        // Add activity to the database
        if (context.User?.Identity is ZapMeIdentity identity)
        {
            await dbContext.Users
                .Where(s => s.Id == identity.UserId)
                .ExecuteUpdateAsync(spc => spc
                    .SetProperty(u => u.LastOnline, _ => DateTime.UtcNow)
                    );
        }

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}
