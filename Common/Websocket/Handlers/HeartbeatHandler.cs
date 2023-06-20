using client.fbs;
using server.fbs;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        _lastHeartbeat = DateTime.UtcNow;
        ServerHeartbeat response = new ServerHeartbeat
        {
            Timestamp = DateTime.UtcNow.Ticks
        };

        return Task.FromResult(true);
    }
}