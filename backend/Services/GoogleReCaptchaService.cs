using ZapMe.DTOs;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class GoogleReCaptchaService : IGoogleReCaptchaService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GoogleReCaptchaService> _logger;
    private readonly string _reCaptchaSecret;

    public GoogleReCaptchaService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GoogleReCaptchaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _reCaptchaSecret = configuration["Authorization:ReCaptcha:Secret"] ?? throw new KeyNotFoundException("Config entry \"Authorization:ReCaptcha:Secret\" is missing!");
    }

    public async Task<GoogleReCaptchaVerifyResponse> VerifyUserResponseTokenAsync(string reCaptchaToken, string? remoteIpAddress, CancellationToken cancellationToken = default)
    {
#if DEBUG
        if (reCaptchaToken == "skip") return new GoogleReCaptchaVerifyResponse { Success = true };
#endif
        if (String.IsNullOrEmpty(reCaptchaToken))
        {
            return new GoogleReCaptchaVerifyResponse { ErrorCodes = new[] { "invalid-input-response" } };
        }

        Dictionary<string, string> formUrlValues = new Dictionary<string, string>
        {
            { "secret", _reCaptchaSecret },
            { "response", reCaptchaToken }
        };

        if (!String.IsNullOrEmpty(remoteIpAddress))
        {
            formUrlValues.Add("remoteip", remoteIpAddress);
        }

        var httpContent = new FormUrlEncodedContent(formUrlValues);

        HttpClient httpClient = _httpClientFactory.CreateClient("GoogleReCaptcha");

        using HttpResponseMessage response = await httpClient.PostAsync("siteverify", httpContent, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GoogleReCaptchaVerifyResponse>(cancellationToken: cancellationToken);
    }
}
