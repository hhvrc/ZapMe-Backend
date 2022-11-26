using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZapMe.Controllers;
using ZapMe.Middlewares;

// ########################################
// ######## GLOBAL SETTINGS ###############
// ########################################

// Set global Regex timeout to 1 second
AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(1000));

// ########################################
// ######## CREATE BUILDER ################
// ########################################

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment env = builder.Environment;
ILoggingBuilder logging = builder.Logging;
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;
bool isDevelopment = env.IsDevelopment();

// ########################################
// ######## CONFIGURE LOGGING #############
// ########################################

logging.ZMAddLogging();

// ########################################
// ######## CORE SERVICES #################
// ########################################

services.AddRouting();
services.AddControllers().AddJsonOptions(static opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)));
services.AddResponseCompression();
services.Configure<ApiBehaviorOptions>(static opt => opt.InvalidModelStateResponseFactory = ErrorResponseFactory.CreateErrorResult);
//services.AddHealthChecks().AddCheck("sql" ) //TODO: explore this

// ########################################
// ######## ZAPME SERVICES ################
// ########################################

services.AddZapMeServices(); // TODO: bad naming
services.ZMAddRateLimiter();
services.ZMAddSwagger();
services.ZMAddAuthentication(configuration);
services.ZMAddAuthorization();
services.ZMAddDatabase(configuration);
services.ZMAddDataCaching(configuration);
services.ZMAddAmazonAWS(configuration);

// ########################################
// ######## CORS CONFIGURATION ############
// ########################################

if (isDevelopment)
{
    services.ZMAddDevelopment();
}

// ########################################
// ######## BUILD APP #####################
// ########################################

WebApplication app = builder.Build();

// ########################################
// ######## MIDDLEWARE PIPELINE ###########
// ########################################

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseForwardedHeaders();

if (!isDevelopment)
{
    app.UseHsts();
}

//app.UseHttpsRedirection(); // Bad if behind localhost proxy

app.UseResponseCompression();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

if (isDevelopment)
{
    app.UseCors("dev");
}

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter(); // As early as possible
// App!.UseHealthChecks("/api/v1/health/"); // TODO: explore this

app.UseSwaggerAndUI();
app.UseMiddleware<ActivityTracker>();

app.MapControllers();

// ########################################
// ######## RUN APP #######################
// ########################################

app.Run();