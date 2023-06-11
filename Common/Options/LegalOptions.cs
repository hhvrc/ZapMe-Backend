namespace ZapMe.Options;

public sealed class LegalOptions
{
    public const string SectionName = "Legal";

    public required uint PrivacyPolicyVersion { get; set; }
    public required uint TermsOfServiceVersion { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LegalOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}