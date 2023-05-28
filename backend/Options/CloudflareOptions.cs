namespace ZapMe.Options;

public sealed class CloudflareOptions
{
    public const string SectionName = "Cloudflare";

    public required CloudflareR2Options R2 { get; set; }
    public required CloudflareTurnstileOptions Turnstile { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
        CloudflareR2Options.Register(services, configuration);
        CloudflareTurnstileOptions.Register(services, configuration);

    }
}

public sealed class CloudflareR2Options
{
    public const string SectionName = CloudflareOptions.SectionName + ":R2";
    
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string ServiceURL { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareR2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
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