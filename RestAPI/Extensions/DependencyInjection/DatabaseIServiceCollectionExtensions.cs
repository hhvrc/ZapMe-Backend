using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DatabaseIServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        DatabaseOptions options = configuration.GetRequiredSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()!;

        return services.AddDbContextPool<ZapMeContext>(dbOpt =>
            {
                dbOpt.UseNpgsql(options.ConnectionString, npgSqlOpt =>
                {
                    npgSqlOpt.SetPostgresVersion(options.ServerVersionMajor, options.ServerVersionMinor);
                });
                dbOpt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
    }
}
