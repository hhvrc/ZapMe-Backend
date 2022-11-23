namespace ZapMe.App;

partial class App
{
    private void AddDataCaching()
    {
        Services.AddMemoryCache();
        Services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = Configuration["Redis:ConnectionString"];
        });
    }
}
