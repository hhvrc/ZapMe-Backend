namespace ZapMe.Options;

public sealed class CloudflareOptions
{
    public const string SectionName = "Cloudflare";

    public required CloudflareTurnstileOptions Turnstile { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        CloudflareTurnstileOptions.Register(services, configuration);

    }
}

public sealed class CloudflareTurnstileOptions
{
    public const string SectionName = CloudflareOptions.SectionName + ":Turnstile";

    public required string SiteKey { get; set; }
    public required string SecretKey { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareTurnstileOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
    }
}