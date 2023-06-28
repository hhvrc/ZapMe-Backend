using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZapMe.Options;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public class CloudflareR2Service : ICloudflareR2Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<CloudflareR2Service> _logger;

    public CloudflareR2Service(IOptions<CloudflareR2Options> options, ILogger<CloudflareR2Service> logger)
    {
        CloudflareR2Options optionsValue = options.Value;
        BasicAWSCredentials credentials = new BasicAWSCredentials(optionsValue.AccessKey, optionsValue.SecretKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = optionsValue.ServiceURL,
        });
        _logger = logger;
    }

    public Task UploadObjectAsync(PutObjectRequest request, CancellationToken cancellationToken)
    {
        return _s3Client.PutObjectAsync(request, cancellationToken);
    }

    public Task DeleteObjectAsync(DeleteObjectRequest request, CancellationToken cancellationToken)
    {
        return _s3Client.DeleteObjectAsync(request, cancellationToken);
    }
}
