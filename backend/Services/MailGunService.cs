using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Config.Models;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public static class MailGunServiceExtensions
{
    public static IServiceCollection AddMailGunService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(MailGunService.HttpClientKey, cli =>
        {
            MailGunOptions options = configuration.GetRequiredSection(MailGunOptions.SectionName).Get<MailGunOptions>()!;

            cli.BaseAddress = new Uri(MailGunService.BaseUrl, UriKind.Absolute);
            cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
            cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{options.ApiKey}")));
        });
        services.AddSingleton<IMailGunService, MailGunService>();
        return services;
    }
}

public sealed class MailGunService : IMailGunService
{
    public const string HttpClientKey = "MailGun";
    public const string BaseUrl = "https://api.eu.mailgun.net/v3/";
    public const string SendMailEndpoint = "messages";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LockOutStore> _logger;

    public MailGunService(IHttpClientFactory httpClientFactory, ILogger<LockOutStore> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private async Task<bool> SendEmailAsync(string senderName, string recepient, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        if (!EmailUtils.IsValid(recepient))
        {
            _logger.LogError("Failed to parse recepient email: {}", recepient);
            return false;
        }

        string sender = $"{senderName} <{senderName.ToLower()}@{App.Domain}>";
        if (!EmailUtils.IsValid(sender))
        {
            _logger.LogError("Failed to parse sender email: {}", sender);
            return false;
        }

        MultipartFormDataContent content = new MultipartFormDataContent
        {
            { new StringContent(sender), "from" },
            { new StringContent(recepient), "to" },
            { new StringContent(subject), "subject" },
            { new StringContent(htmlbody), "html" }
        };

        HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientKey);

        using HttpResponseMessage result = await httpClient.PostAsync(Uri.EscapeDataString(App.Domain) + "/" + SendMailEndpoint, content, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send mail: {} {}", result.StatusCode, await result.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }

        return true;
    }

    public Task<bool> SendEmailAsync(string senderName, string recepientName, string recepientEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            $"{recepientName} <{recepientEmail}>",
            subject,
            htmlBody,
            cancellationToken
        );
    }

    public Task<bool> SendEmailAsync(string senderName, MailAddress recepient, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            recepient.ToString(),
            subject,
            htmlbody,
            cancellationToken
        );
    }
}
