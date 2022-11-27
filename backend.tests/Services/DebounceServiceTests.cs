using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using ZapMe.Logic;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Tests.Services;

public sealed class DebounceServiceTests
{
    private readonly IDebounceService _sut;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<DebounceService>> _loggerMock = new();
    private readonly MockHttpMessageHandler _handler = new MockHttpMessageHandler();

    public DebounceServiceTests()
    {
        _httpClient = new HttpClient(_handler);
        _sut = new DebounceService(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task CheckDebounce_GoodEmail()
    {
        string email = "user.name@gmail.com";
        string anonymousEmail = Transformers.AnonymizeEmailUser(email);

        KeyValuePair<string, string>[] query = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("email", anonymousEmail)
        };

        _handler
            .When(HttpMethod.Get, "https://disposable.debounce.io")
            .WithExactQueryString(query)
            .Respond(Application.Json,
/* lang=json */
"""
{
    "disposable": false
}
"""
            );

        Assert.False(await _sut.IsDisposableEmailAsync(email));
    }

    [Fact]
    public async Task CheckDebounce_BadEmail()
    {
        string email = "user.name@disposeme.com";
        string anonymousEmail = Transformers.AnonymizeEmailUser(email);

        KeyValuePair<string, string>[] query = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("email", anonymousEmail)
        };

        _handler
            .When(HttpMethod.Get, "https://disposable.debounce.io")
            .WithExactQueryString(query)
            .Respond(Application.Json,
/* lang=json */
"""
{
    "disposable": "true"
}
"""
            );

        Assert.True(await _sut.IsDisposableEmailAsync(email));
    }
}