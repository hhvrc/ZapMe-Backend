using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class DatabaseIServiceCollectionExtensions
{
    public static void ZMAddDatabase([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services.AddDbContextPool<ZapMeContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetOrThrow("PgSQL:ConnectionString"), o => o.SetPostgresVersion(14, 5))
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }
}
