using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using ZapMe.DTOs;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Tests.Services;

public sealed class GoogleReCaptchaServiceTest
{
    private readonly IGoogleReCaptchaService _sut;
    private readonly HttpClient _httpClient;
    private readonly string _reCaptchaSecret;
    private readonly IConfiguration _configuration;
    private readonly Bogus.Faker _faker = new Bogus.Faker();
    private readonly Mock<ILogger<GoogleReCaptchaService>> _loggerMock = new();
    private readonly MockHttpMessageHandler _handler = new MockHttpMessageHandler();

    public GoogleReCaptchaServiceTest()
    {
        _reCaptchaSecret = _faker.Random.AlphaNumeric(32);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new KeyValuePair<string, string?>[] {
                new("Authorization:ReCaptcha:Secret", _reCaptchaSecret)
            })
            .Build();

        _httpClient = new HttpClient(_handler);
        _sut = new GoogleReCaptchaService(_httpClient, _configuration, _loggerMock.Object);
    }

    private async Task<GoogleReCaptchaVerifyResponse> CreateRequest(string userResponseToken, string remoteIp, string responseBody)
    {
        responseBody = responseBody.Replace("{DateTime}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        _handler
            .When(HttpMethod.Post, "https://www.google.com/recaptcha/api/siteverify")
            .With(req => req.Content?.Headers?.ContentType?.MediaType == "application/x-www-form-urlencoded")
            .WithExactFormData(new KeyValuePair<string, string?>[] {
                new("secret", _reCaptchaSecret),
                new("response", userResponseToken),
                new("remoteip", remoteIp)
            })
            .Respond(Application.Json, responseBody);

        return await _sut.VerifyUserResponseTokenAsync(userResponseToken, remoteIp);
    }

    [Fact]
    public async Task GoodToken()
    {
        string remoteIp = _faker.Internet.Ip();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string body =
/* lang=json */
"""
{
    "success": true,
    "challenge_ts": "{DateTime}",
    "hostname": "mysite.com"
}
""";

        GoogleReCaptchaVerifyResponse response = await CreateRequest(userResponseToken, remoteIp, body);

        Assert.True(response.Success);
        Assert.Equal("mysite.com", response.Hostname);
        Assert.Null(response.ApkPackageName);
        Assert.Null(response.ErrorCodes);
    }

    [Fact]
    public async Task GoodTokenAndroid()
    {
        string remoteIp = _faker.Internet.Ip();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string body =
/* lang=json */
"""
{
    "success": true,
    "challenge_ts": "{DateTime}",
    "apk_package_name": "com.hhvrc_vr.zapme.app"
}
""";

        GoogleReCaptchaVerifyResponse response = await CreateRequest(userResponseToken, remoteIp, body);

        Assert.True(response.Success);
        Assert.Null(response.Hostname);
        Assert.Equal("com.hhvrc_vr.zapme.app", response.ApkPackageName);
        Assert.Null(response.ErrorCodes);
    }

    [Fact]
    public async Task BadToken_MissingInputSecret()
    {
        string remoteIp = _faker.Internet.Ip();
        string userResponseToken = _faker.Random.AlphaNumeric(32);
        string body =
/* lang=json */
"""
{
    "success": false,
    "challenge_ts": "{DateTime}",
    "hostname": "mysite.com",
    "error-codes": [
        "missing-input-secret",
        "invalid-input-secret",
        "missing-input-response",
        "invalid-input-response",
        "bad-request",
        "timeout-or-duplicate"
    ]
}
""";

        GoogleReCaptchaVerifyResponse response = await CreateRequest(userResponseToken, remoteIp, body);

        Assert.False(response.Success);
        Assert.Equal("mysite.com", response.Hostname);
        Assert.Null(response.ApkPackageName);
        Assert.NotNull(response.ErrorCodes);
        Assert.Contains("missing-input-secret", response.ErrorCodes);
        Assert.Contains("invalid-input-secret", response.ErrorCodes);
        Assert.Contains("missing-input-response", response.ErrorCodes);
        Assert.Contains("invalid-input-response", response.ErrorCodes);
        Assert.Contains("bad-request", response.ErrorCodes);
        Assert.Contains("timeout-or-duplicate", response.ErrorCodes);
    }
}
