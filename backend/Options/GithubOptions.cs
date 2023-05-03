namespace ZapMe.Options;

public sealed class GithubOptions
{
    public static string SectionName = "Github";

    public required GithubOAuth2Options OAuth2 { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GithubOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        GithubOAuth2Options.Register(services, configuration);
    }
}

public sealed class GithubOAuth2Options
{
    public static string SectionName = GithubOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }
    public required string RedirectUri { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GithubOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
