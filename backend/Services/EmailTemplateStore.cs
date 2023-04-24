using Amazon.S3;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class EmailTemplateStore : IEmailTemplateStore
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<EmailTemplateStore> _logger;

    public EmailTemplateStore(IAmazonS3 s3Client, ILogger<EmailTemplateStore> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
    }

    public Task<OneOf<string[], ErrorDetails>> GetTemplateNamesAsync(CancellationToken cancellationToken)
    {
        // TODO: move bucket name to config
        return _s3Client.ListObjectsAsync("zapme-email-templates", cancellationToken).ContinueWith<OneOf<string[], ErrorDetails>>(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Error getting template list from S3");
                return CreateHttpError.InternalServerError();
            }

            return t.Result.S3Objects.Select(o => o.Key).ToArray();
        });
    }

    public Task<OneOf<string, ErrorDetails>> GetTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        // TODO: move bucket name to config
        return _s3Client.GetObjectAsync("zapme-email-templates", templateName, cancellationToken).ContinueWith<OneOf<string, ErrorDetails>>(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Error getting template {templateName} from S3", templateName);
                return CreateHttpError.InternalServerError();
            }

            using StreamReader reader = new StreamReader(t.Result.ResponseStream);
            return reader.ReadToEnd();
        });
    }
}
