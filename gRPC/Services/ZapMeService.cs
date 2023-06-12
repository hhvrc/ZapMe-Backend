using Grpc.Core;

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

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    public override Task<WebRtcConnectReply> WebRtcConnect(WebRtcConnectRequest request, ServerCallContext context)
    {
        _logger.LogInformation("WebRtcConnect");

        return Task.FromResult(new WebRtcConnectReply
        {
            SentOffer = true,
            RequestExpiresAt = DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeSeconds()
        });
    }
}
