using ZapMe.Authentication;
using ZapMe.Services.Interfaces;

namespace ZapMe.Middlewares;

public sealed class ActivityTracker
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityTracker> _logger;

    public ActivityTracker(RequestDelegate next, ILogger<ActivityTracker> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUserManager userManager)
    {
        ZapMeIdentity? identity = context.User?.Identity as ZapMeIdentity;

        try
        {
            await _next(context);
        }
        finally
        {
            if (identity != null)
            {
                await userManager.SetLastOnlineAsync(identity.UserId, DateTime.UtcNow, context.RequestAborted);
            }
        }
    }
}
