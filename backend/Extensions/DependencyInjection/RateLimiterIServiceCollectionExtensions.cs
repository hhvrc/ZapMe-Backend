using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using ZapMe.Authentication;
using ZapMe.Data.Models;

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
                SessionEntity? session = (ctx.User as ZapMePrincipal)?.SessionEntity;

                if (session == null)
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

                AccountEntity user = session.User;
                if (user?.UserRoles?.Select(r => r.RoleName).Contains("admin") ?? false)
                {
                    return RateLimitPartition.GetNoLimiter("admin");
                }

                return RateLimitPartition.GetTokenBucketLimiter(session.UserId.ToString(), key => new TokenBucketRateLimiterOptions()
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
