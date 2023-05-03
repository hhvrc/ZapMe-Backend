namespace ZapMe.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string ConnectionString { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
