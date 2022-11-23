using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using ZapMe.Logic;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public sealed class DebounceService : IDebounceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DebounceService> _logger;

    public DebounceService(HttpClient httpClient, ILogger<DebounceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Set client defaults
        _httpClient.BaseAddress = new Uri($"https://disposable.debounce.io", UriKind.Absolute);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
    }

    private struct DebounceDisposableResponse
    {
        [JsonPropertyName("disposable")]
        public string Disposable { get; set; }
    }

    public async Task<bool> IsDisposableEmailAsync(string email, CancellationToken cancellationToken)
    {
        string? anonymizedEmail = Transformers.AnonymizeEmailUser(email);
        if (anonymizedEmail == null)
        {
            return false;
        }

        bool isDisposable;

        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("?email=" + anonymizedEmail, cancellationToken);

            DebounceDisposableResponse content = await response.Content.ReadFromJsonAsync<DebounceDisposableResponse>(cancellationToken: cancellationToken);

            isDisposable = Boolean.Parse(content.Disposable);
        }
        catch (Exception)
        {
            isDisposable = false;
            _logger.LogError("disposable.io sent back invalid return for {}", anonymizedEmail);
        }

        return isDisposable;
    }
}
