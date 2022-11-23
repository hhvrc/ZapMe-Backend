using Microsoft.EntityFrameworkCore;

namespace ZapMe.App;

partial class App
{
    private void AddDatabase()
    {
        Services.AddDbContextPool<ZapMe.Data.ZapMeContext>(opt =>
        {
            opt.UseNpgsql(Configuration["PgSQL:ConnectionString"], o => o.SetPostgresVersion(14, 5))
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }
}
