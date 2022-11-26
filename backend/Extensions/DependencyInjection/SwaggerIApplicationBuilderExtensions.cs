using System.Diagnostics.CodeAnalysis;
using ZapMe;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerIApplicationBuilderExtensions
{
    public static void UseSwaggerAndUI([NotNull] this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", Constants.AppName + " API - json");
            opt.SwaggerEndpoint("/swagger/v1/swagger.yaml", Constants.AppName + " API - yaml");
        });
    }
}
