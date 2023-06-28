using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZapMe.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string SigningKey { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>().Bind(configuration.GetRequiredSection(SectionName));
    }
}