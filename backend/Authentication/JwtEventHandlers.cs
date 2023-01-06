using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Authentication;

public static class JwtEventHandlers
{
    public static Task OnChallenge(JwtBearerChallengeContext ctx)
    {
        return Task.CompletedTask;
    }
    internal static Task OnForbidden(ForbiddenContext ctx)
    {
        return Task.CompletedTask;
    }
    public static async Task OnTokenValidated(TokenValidatedContext ctx)
    {
        ClaimsPrincipal? principal = ctx.Principal;
        if (principal == null)
        {
            return;
        }

        if (!Guid.TryParse(principal.FindFirst(ZapMeIdentity.SessionIdClaimType)?.Value, out Guid sessionId))
        {
            ctx.Fail("Invalid session id.");
            return;
        }

        HttpContext httpContext = ctx.HttpContext;
        ISessionManager sessionManager = httpContext.RequestServices.GetRequiredService<ISessionManager>();
        SessionEntity? sessionEntity = await sessionManager.SessionStore.GetByIdAsync(sessionId, ctx.HttpContext.RequestAborted);

        if (sessionEntity == null)
        {
            ctx.Fail("Invalid session.");
            return;
        }

        if (sessionEntity.IsExpired)
        {
            ctx.Fail("Session expired.");
            return;
        }

        ctx.Principal = new ZapMePrincipal(sessionEntity);
    }

    internal static Task OnAuthenticationFailed(AuthenticationFailedContext ctx)
    {
        return Task.CompletedTask;
    }
}
