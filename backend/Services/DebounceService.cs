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
        public bool IsDisposable { get; set; }
    }

    public async Task<bool> IsDisposableEmailAsync(string email, CancellationToken cancellationToken)
    {
        string? anonymizedEmail = Transformers.AnonymizeEmailUser(email);
        if (anonymizedEmail == null)
        {
            return false;
        }

        using HttpResponseMessage response = await _httpClient.GetAsync("?email=" + anonymizedEmail, cancellationToken);

        DebounceDisposableResponse content = await response.Content.ReadFromJsonAsync<DebounceDisposableResponse>(cancellationToken: cancellationToken);

        return content.IsDisposable;
    }
}
