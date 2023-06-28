using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZapMe.Database.Extensions;

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
            });
    }
}
