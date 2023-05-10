using Microsoft.OpenApi.Models;
using ZapMe.Constants;
using ZapMe.Swagger;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerIServiceCollectionExtensions
{
    public static void ZMAddSwagger(this IServiceCollection services, bool isDev)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SchemaFilter<AttributeFilter>();
            opt.ParameterFilter<AttributeFilter>();
            opt.OperationFilter<AttributeFilter>();
            opt.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
            opt.SupportNonNullableReferenceTypes(); // Sets Nullable flags appropriately.
            //opt.UseAllOfToExtendReferenceSchemas(); // Allows $ref enums to be nullable
            //opt.UseAllOfForInheritance(); // Allows $ref objects to be nullable

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
                },
            });
            if (isDev)
            {
                opt.AddServer(new OpenApiServer { Url = "http://localhost:5296" });
                opt.AddServer(new OpenApiServer { Url = "https://localhost:7296" });
            }
            else
            {
                opt.AddServer(new OpenApiServer { Url = App.BackendUrl });
            }

            opt.SupportNonNullableReferenceTypes();

            opt.IncludeXmlComments(App.AssemblyXmlPath);
        });
    }
}
