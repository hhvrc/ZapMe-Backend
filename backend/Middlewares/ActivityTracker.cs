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

    public async Task InvokeAsync(HttpContext context, ZapMeContext dbContext, CancellationToken cancellationToken)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            if (context.User?.Identity is ZapMeIdentity identity)
            {
                await dbContext.Users.Where(s => s.Id == identity.UserId).ExecuteUpdateAsync(spc => spc.SetProperty(u => u.LastOnline, _ => DateTime.UtcNow), cancellationToken);
            }
        }
    }
}
