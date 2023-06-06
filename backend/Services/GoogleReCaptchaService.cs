using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public static class GoogleReCaptchaServiceExtensions
{
    public static IServiceCollection AddGoogleReCaptchaService(this IServiceCollection services, IConfiguration configuration)
    {
        GoogleReCaptchaOptions.Register(services, configuration);
        services.AddHttpClient(GoogleReCaptchaService.HttpClientKey, cli =>
        {
            cli.BaseAddress = new Uri(GoogleReCaptchaService.BaseUrl, UriKind.Absolute);
            cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        services.AddTransient<IGoogleReCaptchaService, GoogleReCaptchaService>();
        return services;
    }
}

public sealed class GoogleReCaptchaService : IGoogleReCaptchaService
{
    public const string HttpClientKey = "GoogleReCaptcha";
    public const string BaseUrl = "https://www.google.com/recaptcha/api/";
    public const string SiteVerifyEndpoint = "siteverify";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleReCaptchaOptions _options;
    private readonly ILogger<GoogleReCaptchaService> _logger;

    public GoogleReCaptchaService(IHttpClientFactory httpClientFactory, IOptions<GoogleReCaptchaOptions> options, ILogger<GoogleReCaptchaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<GoogleReCaptchaVerifyResponse> VerifyUserResponseTokenAsync(string responseToken, string? remoteIpAddress, CancellationToken cancellationToken)
    {
#if DEBUG
        if (responseToken == "skip") return new GoogleReCaptchaVerifyResponse { Success = true };
#endif
        if (String.IsNullOrEmpty(responseToken))
        {
            return new GoogleReCaptchaVerifyResponse { ErrorCodes = new[] { "invalid-input-response" } };
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

        var httpContent = new FormUrlEncodedContent(formUrlValues);

        HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientKey);

        using HttpResponseMessage response = await httpClient.PostAsync(SiteVerifyEndpoint, httpContent, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GoogleReCaptchaVerifyResponse>(cancellationToken: cancellationToken);
    }
}
