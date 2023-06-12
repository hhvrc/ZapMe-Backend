namespace ZapMe.gRPC.Services;

public class ZapMeGrpcServiceImpl : ZapMeGrpcService.ZapMeGrpcServiceBase
{
    private readonly ILogger<ZapMeGrpcServiceImpl> _logger;

    public ZapMeGrpcServiceImpl(ILogger<ZapMeGrpcServiceImpl> logger)
    {
        _logger = logger;
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}
