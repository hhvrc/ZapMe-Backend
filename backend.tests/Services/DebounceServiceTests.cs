using Microsoft.Extensions.Logging;
using Moq.Protected;
using RichardSzalay.MockHttp;
using System.Net;
using ZapMe.Logic;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Tests.Services;

public sealed class DebounceServiceTests
{
    private readonly IDebounceService _sut;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Mock<ILogger<DebounceService>> _loggerMock = new();
    private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();
    
    public DebounceServiceTests()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(_handlerMock) { BaseAddress = new Uri("https://disposable.debounce.io", UriKind.Absolute) });

        _httpClientFactory = httpClientFactoryMock.Object;
        _sut = new DebounceService(httpClientFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task IsDisposableEmailAsync_GoodEmail_ReturnsFalse()
    {
        // Arrange
        var email = "user.name@gmail.com";
        var query = new KeyValuePair<string, string>[] {
            new ( "email", Transformers.AnonymizeEmailUser(email) )
        };

        _handlerMock
            .When(HttpMethod.Get, "https://disposable.debounce.io")
            .WithExactQueryString(query)
            .Respond(Application.Json, @"{""disposable"":false}");

        // Act
        var result = await _sut.IsDisposableEmailAsync(email);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsDisposableEmailAsync_BadEmail_ReturnsTrue()
    {
        // Arrange
        var email = "user.name@disposeme.com";
        var query = new KeyValuePair<string, string>[] {
            new ( "email", Transformers.AnonymizeEmailUser(email) )
        };

        _handlerMock
            .When(HttpMethod.Get, "https://disposable.debounce.io")
            .WithExactQueryString(query)
            .Respond(Application.Json, @"{""disposable"":true}");

        // Act
        var result = await _sut.IsDisposableEmailAsync(email);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsDisposableEmailAsync_EmptyResponse_ReturnsFalse()
    {
        // Arrange
        var email = "user.name@disposeme.com";
        
        _handlerMock
            .When(HttpMethod.Get, "https://disposable.debounce.io")
            .Respond(Application.Json, "");

        // Act
        var result = await _sut.IsDisposableEmailAsync(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsDisposableEmailAsync_NetworkError_ReturnsFalse()
    {
        // Arrange
        var email = "user.name@disposeme.com";

        _handlerMock
            .When(HttpMethod.Get, "https://disposable.debounce.io")
            .WithQueryString("email", Transformers.AnonymizeEmailUser(email))
            .Respond(HttpStatusCode.InternalServerError);

        // Act
        var result = await _sut.IsDisposableEmailAsync(email);

        // Assert
        Assert.False(result);
    }
}