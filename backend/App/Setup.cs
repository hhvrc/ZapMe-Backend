using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZapMe.Controllers;

namespace ZapMe.App;

partial class App
{
    private void BuildServiceCollection()
    {
        AddLogging();

        Services.AddRouting();
        Services.AddControllers().AddJsonOptions(static opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)));
        Services.AddResponseCompression();

        Services.Configure<ApiBehaviorOptions>(static opt => opt.InvalidModelStateResponseFactory = ErrorResponseFactory.CreateErrorResult);

        AddCustomServices();

        //TODO: explore this
        //Services.AddHealthChecks().AddCheck("sql" )

        AddRateLimiting();
        AddSwagger();
        AddAuthentication();
        AddAuthorization();
        AddDatabase();
        AddDataCaching();
        AddAmazonAWS();

        if (IsDevelopment)
        {
            IdentityModelEventSource.ShowPII = true;
            Services.AddCors(static opt => opt.AddPolicy("dev", static builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
        }

        Application = Builder.Build();
    }

    private void SetupPipeline()
    {
        UseExceptionHandler();
        Application.UseForwardedHeaders();
        UseHSTS();
        UseHttpsRedirection();
        Application.UseResponseCompression();
        UseStaticFiles();
        Application.UseCookiePolicy();
        UseRouting();
        UseCORS();
        UseAuthentication();
        UseAuthorization();
        UseBuiltInMiddlewares();
        UseCustomMiddlewares();
        UseEndpoint();
    }
}
