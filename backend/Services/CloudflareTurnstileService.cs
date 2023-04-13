using ZapMe.DTOs;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class CloudFlareTurnstileService : ICloudFlareTurnstileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CloudFlareTurnstileService> _logger;
    private readonly string _reCaptchaSecret;

    public CloudFlareTurnstileService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CloudFlareTurnstileService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _reCaptchaSecret = configuration["Authorization:CloudflareTurnstile:Secret"] ?? throw new KeyNotFoundException("Config entry \"Authorization:CloudflareTurnstile:Secret\" is missing!");
    }

    public async Task<CloudflareTurnstileVerifyResponse> VerifyUserResponseTokenAsync(string token, string remoteIpAddress, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrEmpty(token))
        {
            return new CloudflareTurnstileVerifyResponse { ErrorCodes = new[] { "invalid-input-response" } };
        }

        Dictionary<string, string> formUrlValues = new Dictionary<string, string>
        {
            { "secret", _reCaptchaSecret },
            { "response", token },
            { "remoteip", remoteIpAddress }
        };

        var httpContent = new FormUrlEncodedContent(formUrlValues);

        HttpClient httpClient = _httpClientFactory.CreateClient("CloudflareTurnstile");

        using HttpResponseMessage response = await httpClient.PostAsync("siteverify", httpContent, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CloudflareTurnstileVerifyResponse>(cancellationToken: cancellationToken);
    }
}
