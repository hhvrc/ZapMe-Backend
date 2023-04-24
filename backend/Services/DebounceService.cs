using System.Text.Json.Serialization;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class DebounceService : IDebounceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DebounceService> _logger;

    public DebounceService(IHttpClientFactory httpClientFactory, ILogger<DebounceService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private struct DebounceDisposableResponse
    {
        [JsonPropertyName("disposable")]
        public string Disposable { get; set; }
    }

    public async Task<bool> IsDisposableEmailAsync(string emailAddress, CancellationToken cancellationToken)
    {
        EmailUtils.ParsedResult parsed = EmailUtils.Parse(emailAddress);
        if (!parsed.Success)
        {
            _logger.LogError("Failed to parse email: {}", emailAddress);
            return false;
        }

        // We want to forward aliases, but not the user part
        // Aliases might help identify if the email address is disposable or not
        // But if we forward the user part and the external service sells the data, user might get spammed, we don't want that
        string query = parsed.HasAlias ? $"?email={parsed.UserAlias}+user@{parsed.Host}" : $"?email=user@{parsed.Host}";


        HttpClient httpClient = _httpClientFactory.CreateClient("Debounce");

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
            _logger.LogError("disposable.io sent back invalid return for {}", emailAddress);
            return false;
        }

        if (!Boolean.TryParse(isDisposableStr, out bool isDisposable))
        {
            _logger.LogError("disposable.io sent back invalid return for {}", emailAddress);
            return false;
        }

        return isDisposable;
    }
}
