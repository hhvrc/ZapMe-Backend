using ZapMe.gRPC.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(opt =>
{
    opt.MaxReceiveMessageSize = 1024 * 1024; // 1MB
    //opt.Interceptors.Add<ZapMeGrpcServiceInterceptor>();
});
builder.Services.AddMediator();

WebApplication app = builder.Build();

app.MapGrpcService<ZapMeGrpcServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
