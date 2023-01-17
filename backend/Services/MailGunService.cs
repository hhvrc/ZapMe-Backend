using ZapMe.Services.Interfaces;

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

    public async Task<bool> SendMailAsync(string senderName, string senderExt, string senderDomain, string recepients, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        MultipartFormDataContent content = new MultipartFormDataContent
        {
            { new StringContent($"{senderName} <{senderExt}@{senderDomain}>"), "from" },
            { new StringContent(recepients), "to" },
            { new StringContent(subject), "subject" },
            { new StringContent(htmlbody), "html" }
        };

        HttpClient httpClient = _httpClientFactory.CreateClient("MailGun");

        using HttpResponseMessage result = await httpClient.PostAsync(Uri.EscapeDataString(senderDomain) + "/messages", content, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send mail: {} {}", result.StatusCode, await result.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }

        return true;
    }
}
