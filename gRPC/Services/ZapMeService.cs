using Grpc.Core;

namespace ZapMe.gRPC.Services;

public class ZapMeGrpcServiceImpl : ZapMeGrpcService.ZapMeGrpcServiceBase
{
    private readonly ILogger<ZapMeGrpcServiceImpl> _logger;
    public ZapMeGrpcServiceImpl(ILogger<ZapMeGrpcServiceImpl> logger)
    {
        _logger = logger;
    }

    public override Task<PingReply> Ping(PingRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Ping");
        return Task.FromResult(new PingReply
        {
            Message = "Pong"
        });
    }
}
