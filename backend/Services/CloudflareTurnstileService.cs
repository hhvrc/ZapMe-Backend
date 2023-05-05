using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public static class CloudflareTurnstileServiceExtensions
{
    public static IServiceCollection AddCloudflareTurnstileService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CloudflareTurnstileOptions>().Bind(configuration.GetRequiredSection(CloudflareTurnstileOptions.SectionName)).ValidateOnStart();
        services.AddHttpClient(CloudflareTurnstileService.HttpClientKey, client =>
        {
            client.BaseAddress = new Uri(CloudflareTurnstileService.BaseUrl, UriKind.Absolute);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        services.AddTransient<ICloudflareTurnstileService, CloudflareTurnstileService>();
        return services;
    }
}

public sealed class CloudflareTurnstileService : ICloudflareTurnstileService
{
    public const string HttpClientKey = "CloudflareTurnstile";
    public const string BaseUrl = "https://challenges.cloudflare.com/turnstile/v0/";
    public const string SiteVerifyEndpoint = "siteverify";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CloudflareTurnstileOptions _options;
    private readonly ILogger<CloudflareTurnstileService> _logger;

    public CloudflareTurnstileService(IHttpClientFactory httpClientFactory, IOptions<CloudflareTurnstileOptions> options, ILogger<CloudflareTurnstileService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CloudflareTurnstileVerifyResponse> VerifyUserResponseTokenAsync(string responseToken, string? remoteIpAddress, CancellationToken cancellationToken)
    {
        if (String.IsNullOrEmpty(responseToken))
        {
            return new CloudflareTurnstileVerifyResponse { ErrorCodes = new[] { "missing-input-response" } };
        }

        Dictionary<string, string> formUrlValues = new Dictionary<string, string>
        {
            { "secret", _options.SecretKey },
            { "response", responseToken }
        };

        if (!String.IsNullOrEmpty(remoteIpAddress))
        {
            formUrlValues.Add("remoteip", remoteIpAddress);
        }

        int retryCount = 0;
        CloudflareTurnstileVerifyResponse response;
        do
        {
            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(formUrlValues);

            HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientKey);

            using HttpResponseMessage httpResponse = await httpClient.PostAsync(SiteVerifyEndpoint, httpContent, cancellationToken);

            response = await httpResponse.Content.ReadFromJsonAsync<CloudflareTurnstileVerifyResponse>(cancellationToken: cancellationToken);
        }
        while (response.ErrorCodes != null && response.ErrorCodes.Contains("internal-error") && retryCount++ < 3);

        return response;
    }
}