using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZapMe.Options;

public sealed class CloudflareR2Options
{
    public const string SectionName = CloudflareOptions.SectionName + ":R2";

    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string ServiceURL { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareR2Options>().Bind(configuration.GetRequiredSection(SectionName));
    }
}