using System.Diagnostics.CodeAnalysis;
using ZapMe.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerIApplicationBuilderExtensions
{
    public static void UseSwaggerAndUI([NotNull] this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", App.AppName + " API - json");
            opt.SwaggerEndpoint("/swagger/v1/swagger.yaml", App.AppName + " API - yaml");
        });
    }
}
