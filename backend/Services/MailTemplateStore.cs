using Amazon.S3;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class MailTemplateStore : IMailTemplateStore
{
    private readonly AmazonS3Client _s3Client;
    private readonly ILogger<MailTemplateStore> _logger;

    public MailTemplateStore(AmazonS3Client s3Client, ILogger<MailTemplateStore> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task<string[]> GetTemplateNamesAsync(CancellationToken cancellationToken)
    {
        // TODO: move bucket name to config
        return await _s3Client.ListObjectsAsync("zapme-email-templates", cancellationToken).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Error getting template list from S3");
                return Array.Empty<string>();
            }

            return t.Result.S3Objects.Select(o => o.Key).ToArray();
        });
    }

    public async Task<string?> GetTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        // TODO: move bucket name to config
        return await _s3Client.GetObjectAsync("zapme-email-templates", templateName, cancellationToken).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Error getting template {templateName} from S3", templateName);
                return null;
            }

            using var reader = new StreamReader(t.Result.ResponseStream);
            return reader.ReadToEnd();
        });
    }
}
