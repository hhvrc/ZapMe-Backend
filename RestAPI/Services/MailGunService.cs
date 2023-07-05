using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using ZapMe.BusinessRules;
using ZapMe.Constants;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
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
    private readonly ILogger<MailGunService> _logger;

    public MailGunService(IHttpClientFactory httpClientFactory, ILogger<MailGunService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private async Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepient, string subject, CancellationToken cancellationToken, params (string nmae, string value)[] additionalContent)
    {
        if (!EmailValidator.IsValid(recepient))
        {
            _logger.LogError("Failed to parse recepient email");
            return false;
        }

        string sender = $"{senderName} <{senderExtension}@{App.Domain}>";
        if (!EmailValidator.IsValid(sender))
        {
            _logger.LogError("Failed to parse sender email");
            return false;
        }

        MultipartFormDataContent content = new MultipartFormDataContent
        {
            { new StringContent(sender), "from" },
            { new StringContent(recepient), "to" },
            { new StringContent(subject), "subject" }
        };

        foreach ((string name, string value) in additionalContent)
        {
            content.Add(new StringContent(value), name);
        }

#if DEBUG
        _logger.LogInformation("Sending mail: {}", await content.ReadAsStringAsync(cancellationToken));
#else
        HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientKey);

        using HttpResponseMessage result = await httpClient.PostAsync(Uri.EscapeDataString(App.Domain) + "/" + SendMailEndpoint, content, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send mail: {} {}", result.StatusCode, await result.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }
#endif

        return true;
    }

    private Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepient, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            senderExtension,
            recepient,
            subject,
            cancellationToken,
            ("html", htmlbody)
        );
    }

    private Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepient, string subject, string templateName, Dictionary<string, string> variables, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            senderExtension,
            recepient,
            subject,
            cancellationToken,
            ("template", templateName),
            ("h:X-Mailgun-Variables", JsonSerializer.Serialize(variables))
        );
    }

    public Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepientName, string recepientEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            $"{recepientName} <{recepientEmail}>",
            subject,
            htmlBody,
            cancellationToken
        );
    }

    public Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepientName, string recepientEmail, string subject, string templateName, Dictionary<string, string> variables, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            senderExtension,
            $"{recepientName} <{recepientEmail}>",
            subject,
            templateName,
            variables,
            cancellationToken
        );
    }

    public Task<bool> SendEmailAsync(string senderName, string senderExtension, MailAddress recepient, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            senderExtension,
            recepient.ToString(),
            subject,
            htmlbody,
            cancellationToken
        );
    }

    public Task<bool> SendEmailAsync(string senderName, string senderExtension, MailAddress recepient, string subject, string templateName, Dictionary<string, string> variables, CancellationToken cancellationToken)
    {
        return SendEmailAsync(
            senderName,
            senderExtension,
            recepient.ToString(),
            subject,
            templateName,
            variables,
            cancellationToken
        );
    }
}
