using fbs.client;
using fbs.server;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private async Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        _lastHeartbeat = DateTime.UtcNow;

        await SendMessageAsync(new ServerPayload(new ServerHeartbeat { HeartbeatIntervalMs = _heartbeatIntervalMs }), cancellationToken);

        return true;
    }
}