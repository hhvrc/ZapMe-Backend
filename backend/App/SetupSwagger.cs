using Microsoft.OpenApi.Models;

namespace ZapMe.App;

partial class App
{
    private void AddSwagger()
    {
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen(opt =>
        {
            opt.SchemaFilter<ZapMe.Attributes.ZapMeAttributeSwaggerFilter>();
            opt.ParameterFilter<ZapMe.Attributes.ZapMeAttributeSwaggerFilter>();
            opt.OperationFilter<ZapMe.Attributes.ZapMeAttributeSwaggerFilter>();

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

    private void UseSwagger()
    {
        Application.UseSwagger();
        Application.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", Constants.AppName + " API - json");
            opt.SwaggerEndpoint("/swagger/v1/swagger.yaml", Constants.AppName + " API - yaml");
        });
    }
}
