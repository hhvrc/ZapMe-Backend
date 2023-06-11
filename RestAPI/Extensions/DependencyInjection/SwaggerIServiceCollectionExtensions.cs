using Microsoft.OpenApi.Models;
using ZapMe.Constants;
using ZapMe.Swagger;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerIServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services, bool isDev)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SchemaFilter<AttributeFilter>();
            opt.ParameterFilter<AttributeFilter>();
            opt.OperationFilter<AttributeFilter>();
            opt.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
            opt.SupportNonNullableReferenceTypes(); // Sets Nullable flags appropriately.
            opt.UseAllOfToExtendReferenceSchemas(); // Allows $ref enums to be nullable
            opt.UseAllOfForInheritance(); // Allows $ref objects to be nullable

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
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.


                      Enter 'Bearer' [space] and then your token in the text input below.
                      

                        Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        },
                        Scheme = "token",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
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

            opt.IncludeXmlComments(AssemblyConstants.XmlPath);
        });
    }
}
