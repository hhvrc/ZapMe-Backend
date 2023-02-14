using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using ZapMe.Authentication;
using ZapMe.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class RateLimiterIServiceCollectionExtensions
{
    public static void ZMAddRateLimiter([NotNull] this IServiceCollection services)
    {
        services.AddRateLimiter(opt =>
        {
            opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                // TODO: Consider rate limiting frontend too
                // Do not rate limit if it is not a api request
                if (!ctx.Request.Path.StartsWithSegments(FrontendConstants.NonFrontendPathSegments))
                {
                    return RateLimitPartition.GetNoLimiter("frontend");
                }

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

                return RateLimitPartition.GetTokenBucketLimiter(identity.AccountId.ToString(), key => new TokenBucketRateLimiterOptions()
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
