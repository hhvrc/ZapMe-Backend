using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using ZapMe.Database;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeDatabaseExtensions
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

    public static async Task<IDbContextTransaction?> BeginTransactionIfNotExistsAsync(this DatabaseFacade db, CancellationToken cancellationToken)
    {
        return db.CurrentTransaction is null ? await db.BeginTransactionAsync(cancellationToken) : null;
    }
}
