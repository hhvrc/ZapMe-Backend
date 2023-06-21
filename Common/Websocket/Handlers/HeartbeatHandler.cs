using fbs.client;
using fbs.server;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        _lastHeartbeat = DateTime.UtcNow;
        ServerHeartbeat response = new ServerHeartbeat
        {
            HeartbeatIntervalMs = 10 * 1000, // 10 seconds TODO: make this configurable
        };

        return Task.FromResult(true);
    }
}