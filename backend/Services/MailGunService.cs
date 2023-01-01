using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Services;

public sealed class MailGunService : IMailGunService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker;
    private readonly ILogger<LockOutStore> _logger;

    public MailGunService(IHttpClientFactory httpClientFactory, ILogger<LockOutStore> logger)
    {        
        _httpClientFactory = httpClientFactory;
        
        _circuitBreaker = Policy<HttpResponseMessage>.Handle<HttpRequestException>().OrTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));
        
        _logger = logger;
    }

    public async Task<bool> SendMailAsync(string senderName, string senderExt, string senderDomain, string recepients, string subject, string htmlbody, CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient("MailGun");

        // Send the request
        using HttpResponseMessage response = await _circuitBreaker.ExecuteAsync(async () =>
            await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Post, HttpUtility.UrlEncode(senderDomain) + "/messages")
                {
                    Content = new MultipartFormDataContent
                    {
                        { new StringContent($"{senderName} <{senderExt}@{senderDomain}>"), "from" },
                        { new StringContent(recepients), "to" },
                        { new StringContent(subject), "subject" },
                        { new StringContent(htmlbody), "html" }
                    }
                },
                cancellationToken
            )
        );
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send mail: {} {}", response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }

        return true;
    }
}