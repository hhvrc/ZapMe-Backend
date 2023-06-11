using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using System.Text.Json;
using ZapMe.Options;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Tests.Services;

public sealed class CloudflareTurnstileServiceTests
{
    private readonly string _reCaptchaSecret;
    private readonly ICloudflareTurnstileService _sut;
    private readonly IOptions<CloudflareTurnstileOptions> _options;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<CloudflareTurnstileService>> _loggerMock = new();
    private readonly MockHttpMessageHandler _httpMessageHandlerMock;
    private readonly Bogus.Faker _faker;

    public CloudflareTurnstileServiceTests()
    {
        _faker = new Bogus.Faker();
        _loggerMock = new Mock<ILogger<CloudflareTurnstileService>>();
        _httpMessageHandlerMock = new MockHttpMessageHandler();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _reCaptchaSecret = _faker.Random.AlphaNumeric(32);

        _options = Microsoft.Extensions.Options.Options.Create(new CloudflareTurnstileOptions
        {
            SiteKey = "",
            SecretKey = _reCaptchaSecret
        });

        _httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(_httpMessageHandlerMock) { BaseAddress = new Uri(CloudflareTurnstileService.BaseUrl, UriKind.Absolute) });

        _sut = new CloudflareTurnstileService(_httpClientFactoryMock.Object, _options, _loggerMock.Object);
    }

    void ArrangeMock(string userResponseToken, string remoteIp, string responseBody)
    {
        _httpMessageHandlerMock
            .When(HttpMethod.Post, CloudflareTurnstileService.BaseUrl + CloudflareTurnstileService.SiteVerifyEndpoint)
            .With(req => req.Content?.Headers?.ContentType?.MediaType == "application/x-www-form-urlencoded")
            .WithExactFormData(new KeyValuePair<string, string?>[] {
                new("secret", _reCaptchaSecret),
                new("response", userResponseToken),
                new("remoteip", remoteIp)
            })
            .Respond(Application.Json, responseBody);
    }
    static string CreateResponseBody(bool success, string hostName, params string[] errorCodes)
    {
        Dictionary<string, object> document = new Dictionary<string, object>
        {
            { "success", success },
            { "challenge_ts", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") },
            { "hostname", hostName }
        };

        if (errorCodes.Length > 0)
        {
            document.Add("error-codes", errorCodes);
        }

        return JsonSerializer.Serialize(document);
    }
    static string CreateResponseBodyApk(bool success, string apkPackageName, params string[] errorCodes)
    {
        Dictionary<string, object> document = new Dictionary<string, object>
        {
            { "success", success },
            { "challenge_ts", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") },
            { "apk_package_name", apkPackageName }
        };

        if (errorCodes.Length > 0)
        {
            document.Add("error-codes", errorCodes);
        }

        return JsonSerializer.Serialize(document);
    }

    [Fact]
    public async Task VerifyUserResponseTokenAsync_GoodToken_ReturnsSuccess()
    {
        // Arrange
        string remoteIp = _faker.Internet.Ip();
        string domainName = _faker.Internet.DomainName();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string responseBody = CreateResponseBody(true, domainName);

        ArrangeMock(userResponseToken, remoteIp, responseBody);

        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(domainName, result.Hostname);
        Assert.Null(result.ErrorCodes);
    }

    [Fact]
    public async Task VerifyUserResponseTokenAsync_GoodAndroidToken_ReturnsSuccess()
    {
        // Arrange
        string remoteIp = _faker.Internet.Ip();
        string apkPackageName = _faker.Internet.DomainName();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string responseBody = CreateResponseBodyApk(true, apkPackageName);

        ArrangeMock(userResponseToken, remoteIp, responseBody);

        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Hostname);
        Assert.Null(result.ErrorCodes);
    }

    [Fact]
    public async Task VerifyUserResponseTokenAsync_BadToken_ReturnsErrorCodes()
    {
        string remoteIp = _faker.Internet.Ip();
        string domainName = _faker.Internet.DomainName();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string responseBody = CreateResponseBody(false, domainName, "missing-input-secret", "invalid-input-secret", "missing-input-response", "invalid-input-response", "bad-request", "timeout-or-duplicate");

        ArrangeMock(userResponseToken, remoteIp, responseBody);

        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(domainName, result.Hostname);
        Assert.NotNull(result.ErrorCodes);
        Assert.Contains("missing-input-secret", result.ErrorCodes);
        Assert.Contains("invalid-input-secret", result.ErrorCodes);
        Assert.Contains("missing-input-response", result.ErrorCodes);
        Assert.Contains("invalid-input-response", result.ErrorCodes);
        Assert.Contains("bad-request", result.ErrorCodes);
        Assert.Contains("timeout-or-duplicate", result.ErrorCodes);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null!)]
    public async Task VerifyUserResponseTokenAsync_NullOrEmptyResponseToken_ReturnsMissingInputResponseErrorCode(string userResponseToken)
    {
        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, "");

        // Assert
        Assert.NotNull(result.ErrorCodes);
        Assert.Contains("missing-input-response", result.ErrorCodes);
    }
}
