using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using ZapMe.Logic;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

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
        string? anonymizedEmail = Transformers.AnonymizeEmailUser(email);
        if (anonymizedEmail == null)
        {
            return false;
        }

        HttpClient client = _httpClientFactory.CreateClient("Debounce");

        using HttpResponseMessage response = await client.GetAsync("?email=" + anonymizedEmail, cancellationToken);

        DebounceDisposableResponse content = await response.Content.ReadFromJsonAsync<DebounceDisposableResponse>(cancellationToken: cancellationToken);

        if (!Boolean.TryParse(content.Disposable, out bool isDisposable))
        {
            _logger.LogError("disposable.io sent back invalid return for {}", anonymizedEmail);
            return false;
        }

        return isDisposable;
    }
}
