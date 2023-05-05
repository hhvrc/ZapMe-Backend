namespace ZapMe.Options;

public sealed class GoogleOptions
{
    public const string SectionName = "Google";

    public required GoogleReCaptchaOptions ReCaptcha { get; set; }

    internal static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GoogleOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        GoogleReCaptchaOptions.Register(services, configuration);
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
