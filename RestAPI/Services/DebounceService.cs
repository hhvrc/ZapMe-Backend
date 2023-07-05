using System.Net.Http.Headers;
using ZapMe.BusinessRules;
using ZapMe.Constants;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public static class DebounceServiceExtensions
{
    public static IServiceCollection AddDebounceService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(DebounceService.HttpClientKey, client =>
        {
            client.BaseAddress = new Uri(DebounceService.BaseUrl, UriKind.Absolute);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        services.AddTransient<IDebounceService, DebounceService>();
        return services;
    }
}

public sealed class DebounceService : IDebounceService
{
    public const string HttpClientKey = "Debounce";
    public const string BaseUrl = "https://disposable.debounce.io/";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DebounceService> _logger;

    public DebounceService(IHttpClientFactory httpClientFactory, ILogger<DebounceService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private struct DebounceDisposableResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string Disposable { get; set; }
    }

    public async Task<bool> IsDisposableEmailAsync(string emailAddress, CancellationToken cancellationToken)
    {
        EmailValidator.ParsedResult parsed = EmailValidator.Parse(emailAddress);
        if (!parsed.Success)
        {
            _logger.LogError("Failed to parse email, aborting");
            return false;
        }

        // We want to forward aliases, but not the user part
        // Aliases might help identify if the email address is disposable or not
        // But if we forward the user part and the external service sells the data, user might get spammed, we don't want that
        string query = parsed.HasAlias ? $"?email={parsed.UserAlias}+user@{parsed.Host}" : $"?email=user@{parsed.Host}";


        HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientKey);

        using HttpResponseMessage response = await httpClient.GetAsync(query, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to check email: {} {}", response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }

        string isDisposableStr;

        try
        {
            DebounceDisposableResponse content = await response.Content.ReadFromJsonAsync<DebounceDisposableResponse>(cancellationToken: cancellationToken);
            isDisposableStr = content.Disposable;
        }
        catch (Exception)
        {
            _logger.LogError("disposable.io sent back invalid return");
            return false;
        }

        if (!Boolean.TryParse(isDisposableStr, out bool isDisposable))
        {
            _logger.LogError("disposable.io sent back invalid return");
            return false;
        }

        return isDisposable;
    }
}
