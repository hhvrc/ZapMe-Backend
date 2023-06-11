using System.Text.Json.Serialization;

namespace ZapMe.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "PgSQL";

    public required string Host { get; set; }
    public required ushort Port { get; set; }
    public required string Database { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required int ServerVersionMajor { get; set; }
    public required int ServerVersionMinor { get; set; }
    public required bool IncludeErrorDetails { get; set; }
    public string? ExtraArguments { get; set; }

    [JsonIgnore]
    public string ConnectionString => $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Include Error Details={IncludeErrorDetails};{ExtraArguments}";

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
