using ZapMe.Data.Models;
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

    public async Task InvokeAsync(HttpContext context, ISignInManager signInManager)
    {
        SignInEntity? signIn = context.GetSignIn();

        try
        {
            await _next(context);
        }
        finally
        {
            if (signIn != null)
            {
                await signInManager.UserManager.SetLastOnlineAsync(signIn.UserId, DateTime.UtcNow, context.RequestAborted);
            }
        }
    }
}
