namespace ZapMe.Options;

public sealed class TwitterOptions
{
    public const string SectionName = "Twitter";

    public required TwitterOAuth2Options OAuth2 { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TwitterOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        TwitterOAuth2Options.Register(services, configuration);
    }
}

public sealed class TwitterOAuth2Options
{
    public const string SectionName = TwitterOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TwitterOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
