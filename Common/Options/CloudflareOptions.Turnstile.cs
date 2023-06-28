using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZapMe.Options;

public sealed class CloudflareTurnstileOptions
{
    public const string SectionName = CloudflareOptions.SectionName + ":Turnstile";

    public required string SiteKey { get; set; }
    public required string SecretKey { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareTurnstileOptions>().Bind(configuration.GetRequiredSection(SectionName));
    }
}