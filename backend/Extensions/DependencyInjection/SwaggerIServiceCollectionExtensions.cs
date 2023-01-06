using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using ZapMe.Attributes;
using ZapMe.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerIServiceCollectionExtensions
{
    public static void ZMAddSwagger([NotNull] this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SchemaFilter<ZapMeAttributeSwaggerFilter>();
            opt.ParameterFilter<ZapMeAttributeSwaggerFilter>();
            opt.OperationFilter<ZapMeAttributeSwaggerFilter>();

            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = App.AppName,
                Description = App.AppDescription,
                TermsOfService = new Uri(App.TermsOfServiceUrl),
                Contact = new OpenApiContact
                {
                    Name = App.ContactText,
                    Url = new Uri(App.ContactUrl)
                },
                License = new OpenApiLicense
                {
                    Name = App.LicenseText,
                    Url = new Uri(App.LicenseUrl)
                }
            });

            opt.SupportNonNullableReferenceTypes();

            opt.IncludeXmlComments(App.AssemblyXmlPath);
        });
    }
}
