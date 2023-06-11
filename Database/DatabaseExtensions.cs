using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZapMe.Database;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeDatabaseIServiceCollectionExtensions
{
    public static IServiceCollection AddZapMeDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetRequiredSection(DatabaseOptions.SectionName);

        services.AddOptions<DatabaseOptions>().Bind(section);

        DatabaseOptions options = section.Get<DatabaseOptions>()!;

        return services.AddDbContextPool<DatabaseContext>(dbOpt =>
            {
                dbOpt.UseNpgsql(options.ConnectionString, npgSqlOpt =>
                {
                    npgSqlOpt.SetPostgresVersion(options.ServerVersionMajor, options.ServerVersionMinor);
                });
                dbOpt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
    }
}
