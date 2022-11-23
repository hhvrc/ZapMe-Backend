using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;

namespace ZapMe.App;

partial class App
{
    private void AddAmazonAWS()
    {
        Services.AddDefaultAWSOptions(new AWSOptions
        {
            Region = RegionEndpoint.GetBySystemName(Configuration["AmazonAWS:Region"]),
            Credentials = new BasicAWSCredentials(Configuration["AmazonAWS:AccessKey"], Configuration["AmazonAWS:SecretKey"])
        });
        Services.AddAWSService<IAmazonS3>();
    }
}