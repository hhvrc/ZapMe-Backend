namespace ZapMe.Services.Interfaces;

public interface IMailGunService
{
    Task<bool> SendEmailAsync(string senderName, string senderExt, string senderDomain, string recepients, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
