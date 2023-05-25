using System.Threading.RateLimiting;
using ZapMe.Authentication;

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
                ZapMeIdentity? identity = (ctx.User as ZapMePrincipal)?.Identity;

                if (identity == null)
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

                if (identity.Roles.Contains("admin"))
                {
                    return RateLimitPartition.GetNoLimiter("admin");
                }

                return RateLimitPartition.GetTokenBucketLimiter(identity.UserId.ToString(), key => new TokenBucketRateLimiterOptions()
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
