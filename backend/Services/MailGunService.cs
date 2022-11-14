using System.Net.Http.Headers;
using System.Text;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public sealed class MailGunService : IMailGunService
{
    private readonly string _domainName;
    private readonly HttpClient _httpClient;
    private readonly ILogger<LockOutStore> _logger;

    public MailGunService(HttpClient httpClient, IConfiguration config, ILogger<LockOutStore> logger)
    {
        _domainName = config["Mailgun:Domain"] ?? throw new NullReferenceException("Config entry \"Mailgun:Domain\" is missing!");
        _httpClient = httpClient;
        _logger = logger;

        string apiKey = config["Mailgun:ApiKey"] ?? throw new NullReferenceException("Config entry \"Mailgun:ApiKey\" is missing!");

        // Set client defaults
        _httpClient.BaseAddress = new Uri($"https://api.eu.mailgun.net/v3/{_domainName}/", UriKind.Absolute);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{apiKey}")));
    }

    public async Task<bool> SendMailAsync(string senderName, string senderExt, string recepients, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        MultipartFormDataContent content = new MultipartFormDataContent
        {
            { new StringContent($"{senderName} <{senderExt}@{_domainName}>"), "from" },
            { new StringContent(recepients), "to" },
            { new StringContent(subject), "subject" },
            { new StringContent(htmlbody), "html" }
        };

        using HttpResponseMessage result = await _httpClient.PostAsync("messages", content, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send mail: {} {}", result.StatusCode, await result.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }

        return true;
    }
}
