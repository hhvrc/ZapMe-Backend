using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using ZapMe;
using ZapMe.Attributes;

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
                Title = Constants.AppName,
                Description = Constants.AppDescription,
                TermsOfService = new Uri(Constants.TermsOfServiceUrl),
                Contact = new OpenApiContact
                {
                    Name = Constants.ContactText,
                    Url = new Uri(Constants.ContactUrl)
                },
                License = new OpenApiLicense
                {
                    Name = Constants.LicenseText,
                    Url = new Uri(Constants.LicenseUrl)
                }
            });

            opt.SupportNonNullableReferenceTypes();

            opt.IncludeXmlComments(Constants.AssemblyXmlPath);
        });
    }
}
