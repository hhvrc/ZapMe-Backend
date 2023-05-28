using Amazon.S3.Model;

namespace ZapMe.Services.Interfaces;

public interface ICloudflareR2Service
{
    Task UploadObjectAsync(PutObjectRequest request, CancellationToken cancellationToken);
    Task DeleteObjectAsync(DeleteObjectRequest request, CancellationToken cancellationToken);
}