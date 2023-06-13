using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database;

namespace ZapMe.Middlewares;

public sealed class ActivityTracker
{
    private readonly RequestDelegate _next;

    public ActivityTracker(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
    {
        // Add TraceIdentifier to the response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["Trace-Id"] = context.TraceIdentifier;
            return Task.CompletedTask;
        });

        // Add activity to the database
        Guid? userId = context.User?.TryGetUserId();
        if (userId.HasValue)
        {
            await dbContext.Users
                .Where(s => s.Id == userId)
                .ExecuteUpdateAsync(spc => spc
                    .SetProperty(u => u.LastOnline, _ => DateTime.UtcNow)
                    );
        }

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}
