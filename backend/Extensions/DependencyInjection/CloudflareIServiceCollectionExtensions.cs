using System.Net.Http.Headers;
using ZapMe.Constants;
using ZapMe.Options;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Extensions.DependencyInjection;

public static class CloudflareIServiceCollectionExtensions
{
    public static IServiceCollection AddCloudflareR2(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareR2Options>().Bind(configuration.GetRequiredSection(CloudflareR2Options.SectionName)).ValidateOnStart();
        return services.AddSingleton<ICloudflareR2Service, CloudflareR2Service>();
    }

    public static IServiceCollection AddCloudflareTurnstileService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareTurnstileOptions>().Bind(configuration.GetRequiredSection(CloudflareTurnstileOptions.SectionName)).ValidateOnStart();
        services.AddHttpClient(CloudflareTurnstileService.HttpClientKey, client =>
        {
            client.BaseAddress = new Uri(CloudflareTurnstileService.BaseUrl, UriKind.Absolute);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        return services.AddTransient<ICloudflareTurnstileService, CloudflareTurnstileService>();
    }
}
