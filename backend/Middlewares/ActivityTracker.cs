using ZapMe.Authentication;
using ZapMe.Services.Interfaces;

namespace ZapMe.Middlewares;

public sealed class ActivityTracker
{
    private readonly RequestDelegate _next;

    public ActivityTracker(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserManager userManager)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            if (context.User?.Identity is ZapMeIdentity identity)
            {
                await userManager.Store.SetLastOnlineAsync(identity.UserId, DateTime.UtcNow, context.RequestAborted);
            }
        }
    }
}
