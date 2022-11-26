using System.Diagnostics.CodeAnalysis;
using ZapMe.Services;
using ZapMe.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataCachingIServiceCollectionExtensions
{
    public static void ZMAddDataCaching([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = configuration["Redis:ConnectionString"];
        });
        services.AddTransient<IHybridCache, HybridCache>();
    }
}
