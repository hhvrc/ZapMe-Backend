using System.Net.Mail;
using ZapMe.Constants;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class MailGunService : IMailGunService
{
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

        HttpClient httpClient = _httpClientFactory.CreateClient("MailGun");

        using HttpResponseMessage result = await httpClient.PostAsync(Uri.EscapeDataString(App.Domain) + "/messages", content, cancellationToken);

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
