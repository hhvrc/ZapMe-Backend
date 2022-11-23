namespace ZapMe.App;

public partial class App
{
    public WebApplicationBuilder Builder { get; private init; }
    public WebApplication Application { get; private set; }

    public ConfigurationManager Configuration => Builder.Configuration;
    public IWebHostEnvironment Environment => Builder.Environment;
    public IServiceCollection Services => Builder.Services;
    public bool IsDevelopment => Environment.IsDevelopment();

    public App(params string[] args)
    {
        Builder = WebApplication.CreateBuilder(args);
        Application = null!;
    }

    public async Task Start()
    {
        await Stop();

        BuildServiceCollection();
        SetupPipeline();

        if (Application == null)
        {
            throw new InvalidOperationException("Application is not initialized");
        }

        Application.Run();
    }

    public async Task Stop()
    {
        if (Application == null) return;

        await Application.StopAsync();
        await Application.DisposeAsync();

        Application = null!;
    }
}
