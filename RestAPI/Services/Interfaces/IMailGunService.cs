using System.Net.Mail;

namespace ZapMe.Services.Interfaces;

public interface IMailGunService
{
    Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepientName, string recepientEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task<bool> SendEmailAsync(string senderName, string senderExtension, string recepientName, string recepientEmail, string subject, string templateName, Dictionary<string, string> variables, CancellationToken cancellationToken = default);
    Task<bool> SendEmailAsync(string senderName, string senderExtension, MailAddress recepient, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task<bool> SendEmailAsync(string senderName, string senderExtension, MailAddress recepient, string subject, string templateName, Dictionary<string, string> variables, CancellationToken cancellationToken = default);
}
