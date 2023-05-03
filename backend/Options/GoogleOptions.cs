namespace ZapMe.Options;

public sealed class GoogleOptions
{
    public const string SectionName = "Google";

    public required GoogleOAuth2Options OAuth2 { get; set; }
    public required GoogleReCaptchaOptions ReCaptcha { get; set; }

    internal static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GoogleOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        GoogleOAuth2Options.Register(services, configuration);
        GoogleReCaptchaOptions.Register(services, configuration);
    }
}

public sealed class GoogleOAuth2Options
{
    public const string SectionName = GoogleOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GoogleOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}

public sealed class GoogleReCaptchaOptions
{
    public const string SectionName = GoogleOptions.SectionName + ":ReCaptcha";

    public required string SiteKey { get; set; }
    public required string SecretKey { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GoogleReCaptchaOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}
