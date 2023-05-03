namespace ZapMe.Options;

public sealed class DiscordOptions
{
    public const string SectionName = "Discord";

    public required DiscordOAuth2Options OAuth2 { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DiscordOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        DiscordOAuth2Options.Register(services, configuration);
    }
}

public sealed class DiscordOAuth2Options
{
    public const string SectionName = DiscordOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }
    public required string RedirectUri { get; set; }
    public required string[] Scopes { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DiscordOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
