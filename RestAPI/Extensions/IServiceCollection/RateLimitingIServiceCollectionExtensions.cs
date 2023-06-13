using System.Threading.RateLimiting;
using ZapMe.Authentication;
using System.Security.Claims;
using ZapMe.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class RateLimitingIServiceCollectionExtensions
{
    public static void AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(opt =>
        {
            opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                Guid? userId = ctx.User.GetUserId();

                bool authenticated = ctx.User.Identity?.IsAuthenticated ?? false;
                if (!authenticated || !userId.HasValue)
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(ctx.GetRemoteIP(), key => new SlidingWindowRateLimiterOptions()
                    {
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        PermitLimit = 120,
                        SegmentsPerWindow = 6,
                        AutoReplenishment = true,
                    });
                }

            if (ctx.User.IsInRole(ZapMeRoleNames.Admin))
                {
                    return RateLimitPartition.GetNoLimiter("admin");
                }

                return RateLimitPartition.GetTokenBucketLimiter(userId.Value.ToString(), key => new TokenBucketRateLimiterOptions()
                {
                    TokenLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 20,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    TokensPerPeriod = 5,
                    AutoReplenishment = true
                });
            });
        });
    }
}
