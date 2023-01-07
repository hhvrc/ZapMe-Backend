using System.Net.Http.Headers;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

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

    private static IEnumerable<KeyValuePair<string, string>> CreateEnumerableKVPair(params (string, string)[] kvPairs)
    {
        foreach ((string key, string value) in kvPairs)
        {
            if (!String.IsNullOrEmpty(value))
            {
                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
    private static FormUrlEncodedContent CreateFormUrlEncodedContent(params (string, string)[] kvPairs)
    {
        return new FormUrlEncodedContent(CreateEnumerableKVPair(kvPairs));
    }

    public async Task<GoogleReCaptchaVerifyResponse> VerifyUserResponseTokenAsync(string reCaptchaToken, string remoteIpAddress, CancellationToken cancellationToken = default)
    {
#if DEBUG
        if (reCaptchaToken == "skip") return new GoogleReCaptchaVerifyResponse { Success = true };
#endif
        if (String.IsNullOrEmpty(reCaptchaToken))
        {
            return new GoogleReCaptchaVerifyResponse { ErrorCodes = new[] { "invalid-input-response" } };
        }

        FormUrlEncodedContent content = CreateFormUrlEncodedContent(
            ("secret", _reCaptchaSecret),
            ("response", reCaptchaToken),
            ("remoteip", remoteIpAddress)
        );

        HttpClient httpClient = _httpClientFactory.CreateClient("GoogleReCaptcha");

        using HttpResponseMessage response = await httpClient.PostAsync("siteverify", content, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GoogleReCaptchaVerifyResponse>(cancellationToken: cancellationToken);
    }
}
