using System.Text.Json.Serialization;
using ZapMe.Logic;
using ZapMe.Services.Interfaces;

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

    public async Task<bool> IsDisposableEmailAsync(string email, CancellationToken cancellationToken)
    {
        string anonymizedEmail = Transformers.AnonymizeEmailUser(email);

        HttpClient httpClient = _httpClientFactory.CreateClient("Debounce");

        using HttpResponseMessage response = await httpClient.GetAsync("?email=" + anonymizedEmail, cancellationToken);

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
            _logger.LogError("disposable.io sent back invalid return for {}", anonymizedEmail);
            return false;
        }

        if (!Boolean.TryParse(isDisposableStr, out bool isDisposable))
        {
            _logger.LogError("disposable.io sent back invalid return for {}", anonymizedEmail);
            return false;
        }

        return isDisposable;
    }
}
