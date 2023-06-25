using Payload = fbs.client.Payload;
using PayloadType = fbs.client.Payload.ItemKind;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> RouteClientMessageAsync(Payload message, CancellationToken cancellationToken)
    {
        return message.Kind switch
        {
            PayloadType.heartbeat => HandleHeartbeatAsync(message.heartbeat, cancellationToken),
            PayloadType.realtime_session => HandleRealtimeSessionAsync(message.realtime_session, cancellationToken),
            PayloadType.NONE => Task.FromResult(false),
            _ => Task.FromResult(false),
        };
    }
}