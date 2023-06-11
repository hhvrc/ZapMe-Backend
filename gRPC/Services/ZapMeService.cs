using Grpc.Core;
using NetMQ;
using NetMQ.Sockets;

namespace ZapMe.gRPC.Services;

public class ZapMeGrpcServiceImpl : ZapMeGrpcService.ZapMeGrpcServiceBase
{
    private readonly SubscriberSocket _netmqSocket;
    private readonly ILogger<ZapMeGrpcServiceImpl> _logger;

    public ZapMeGrpcServiceImpl(ILogger<ZapMeGrpcServiceImpl> logger)
    {
        _netmqSocket = new SubscriberSocket();
        _logger = logger;
    }

    public async Task Initialize()
    {
        // NetMQ configuration
        _netmqSocket.Connect("tcp://localhost:5556");
        _netmqSocket.Subscribe("UserEvent");
        _netmqSocket.Subscribe("GroupEvent");
        _netmqSocket.Subscribe("GlobalEvent");
    }

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    public override Task<WebRtcConnectReply> WebRtcConnect(WebRtcConnectRequest request, ServerCallContext context)
    {
        // Push the request onto the NetMQ queue
        _logger.LogInformation("WebRtcConnect");
        NetMQMessage message = new NetMQMessage();
        message.Append("WebRtcConnect");
        message.Append(request.UserId);
        message.Append(request.OfferType);
        if (request.HasOfferSdp) message.Append(request.OfferSdp);

        // Fire and forget
        _netmqSocket.SendFrame("WebRtcConnect");

        return Task.FromResult(new WebRtcConnectReply
        {
            SentOffer = true,
            RequestExpiresAt = DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeSeconds()
        });
    }
}
