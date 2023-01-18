using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AmazonAWSIServiceCollectionExtensions
{
    public static void ZMAddAmazonAWS([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(new AWSOptions
        {
            Region = RegionEndpoint.GetBySystemName(configuration.GetOrThrow("AmazonAWS:Region")),
            Credentials = new BasicAWSCredentials(configuration.GetOrThrow("AmazonAWS:AccessKey"), configuration.GetOrThrow("AmazonAWS:SecretKey"))
        });
        services.AddAWSService<IAmazonS3>();
    }
}
