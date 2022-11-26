using Microsoft.IdentityModel.Logging;
using ZapMe;

namespace Microsoft.Extensions.DependencyInjection;

public static class DevelopmentIServiceCollectionExtensions
{
    public static void ZMAddDevelopment(this IServiceCollection services)
    {
        IdentityModelEventSource.ShowPII = true;
        services.AddCors(opt =>
        {
            opt.AddPolicy(Constants.DevelopmentCorsPolicyName, policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });
    }
}
