namespace ZapMe.Options;

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
