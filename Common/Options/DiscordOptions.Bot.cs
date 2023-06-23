namespace ZapMe.Options;

public sealed class DiscordBotOptions
{
    public const string SectionName = DiscordOptions.SectionName + ":Bot";

    public required string Token { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DiscordBotOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
