using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZapMe.Options;

public sealed class MailGunOptions
{
    public const string SectionName = "MailGun";

    public required string ApiKey { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MailGunOptions>().Bind(configuration.GetRequiredSection(SectionName));
    }
}