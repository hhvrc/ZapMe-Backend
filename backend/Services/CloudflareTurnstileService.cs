using ZapMe.DTOs;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class CloudFlareTurnstileService : ICloudFlareTurnstileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CloudFlareTurnstileService> _logger;
    private readonly string _siteSecret;

    public CloudFlareTurnstileService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CloudFlareTurnstileService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _siteSecret = configuration["Authorization:CloudflareTurnstile:Secret"] ?? throw new KeyNotFoundException("Config entry \"Authorization:CloudflareTurnstile:Secret\" is missing!");
    }

    public async Task<CloudflareTurnstileVerifyResponse> VerifyUserResponseTokenAsync(string token, string remoteIpAddress, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrEmpty(token))
        {
            return new CloudflareTurnstileVerifyResponse { ErrorCodes = new[] { "missing-input-response" } };
        }

        Dictionary<string, string> formUrlValues = new Dictionary<string, string>
        {
            { "secret", _siteSecret },
            { "response", token },
            { "remoteip", remoteIpAddress }
        };

        int retryCount = 0;
        CloudflareTurnstileVerifyResponse response;
        do
        {
            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(formUrlValues);

            HttpClient httpClient = _httpClientFactory.CreateClient("CloudflareTurnstile");

            using HttpResponseMessage httpResponse = await httpClient.PostAsync("siteverify", httpContent, cancellationToken);

            response = await httpResponse.Content.ReadFromJsonAsync<CloudflareTurnstileVerifyResponse>(cancellationToken: cancellationToken);
        }
        while (response.ErrorCodes != null && response.ErrorCodes.Contains("internal-error") && retryCount++ < 3);

        return response;
    }
}
