using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using ZapMe.DTOs;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Tests.Services;

public sealed class GoogleReCaptchaServiceTests
{
    private readonly string _reCaptchaSecret;
    private readonly IGoogleReCaptchaService _sut;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Mock<ILogger<GoogleReCaptchaService>> _loggerMock = new();
    private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();
    private readonly Bogus.Faker _faker = new Bogus.Faker();

    public GoogleReCaptchaServiceTests()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(_handlerMock) { BaseAddress = new Uri("https://www.google.com/recaptcha/api/", UriKind.Absolute) });

        _reCaptchaSecret = _faker.Random.AlphaNumeric(32);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new KeyValuePair<string, string?>[] {
                new("Authorization:ReCaptcha:Secret", _reCaptchaSecret)
            })
            .Build();

        _httpClientFactory = httpClientFactoryMock.Object;
        _sut = new GoogleReCaptchaService(_httpClientFactory, _configuration, _loggerMock.Object);
    }
    
    void ArrangeMock(string userResponseToken, string remoteIp, string responseBody)
    {
        _handlerMock
            .When(HttpMethod.Post, "https://www.google.com/recaptcha/api/siteverify")
            .With(req => req.Content?.Headers?.ContentType?.MediaType == "application/x-www-form-urlencoded")
            .WithExactFormData(new KeyValuePair<string, string?>[] {
                new("secret", _reCaptchaSecret),
                new("response", userResponseToken),
                new("remoteip", remoteIp)
            })
            .Respond(Application.Json, responseBody);
    }

    [Fact]
    public async Task VerifyUserResponseTokenAsync_GoodToken_ReturnsSuccess()
    {
        // Arrange
        string remoteIp = _faker.Internet.Ip();
        string domainName = _faker.Internet.DomainName();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string responseBody =
        """
        {
            "success": true,
            "challenge_ts": "{DateTime}",
            "hostname": "{DomainName}"
        }
        """
        .Replace("{DateTime}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
        .Replace("{DomainName}", domainName);

        ArrangeMock(userResponseToken, remoteIp, responseBody);
        
        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(domainName, result.Hostname);
        Assert.Null(result.ApkPackageName);
        Assert.Null(result.ErrorCodes);
    }

    [Fact]
    public async Task VerifyUserResponseTokenAsync_GoodAndroidToken_ReturnsSuccess()
    {
        // Arrange
        string remoteIp = _faker.Internet.Ip();
        string apkPackageName = _faker.Internet.DomainName();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string responseBody =
        """
        {
            "success": true,
            "challenge_ts": "{DateTime}",
            "apk_package_name": "{ApkPackageName}"
        }
        """
        .Replace("{DateTime}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
        .Replace("{ApkPackageName}", apkPackageName);

        ArrangeMock(userResponseToken, remoteIp, responseBody);

        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Hostname);
        Assert.Equal(apkPackageName, result.ApkPackageName);
        Assert.Null(result.ErrorCodes);
    }

    [Fact]
    public async Task BadToken_MissingInputSecret()
    {
        string remoteIp = _faker.Internet.Ip();
        string domainName = _faker.Internet.DomainName();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string responseBody =
        """
        {
            "success": false,
            "challenge_ts": "{DateTime}",
            "hostname": "{DomainName}",
            "error-codes": [
                "missing-input-secret",
                "invalid-input-secret",
                "missing-input-response",
                "invalid-input-response",
                "bad-request",
                "timeout-or-duplicate"
            ]
        }
        """
        .Replace("{DateTime}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
        .Replace("{DomainName}", domainName);

        ArrangeMock(userResponseToken, remoteIp, responseBody);

        // Act
        var result = await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(domainName, result.Hostname);
        Assert.Null(result.ApkPackageName);
        Assert.NotNull(result.ErrorCodes);
        Assert.Contains("missing-input-secret", result.ErrorCodes);
        Assert.Contains("invalid-input-secret", result.ErrorCodes);
        Assert.Contains("missing-input-response", result.ErrorCodes);
        Assert.Contains("invalid-input-response", result.ErrorCodes);
        Assert.Contains("bad-request", result.ErrorCodes);
        Assert.Contains("timeout-or-duplicate", result.ErrorCodes);
    }
}
