using fbs.client;
using fbs.server;

namespace ZapMe.Websocket;

partial class WebSocketClient
{
    private async Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        Interlocked.Exchange(ref _lastHeartbeatTicks, DateTimeOffset.UtcNow.Ticks);

        await SendPayloadAsync(new ServerPayload(new ServerHeartbeat
        {
            HeartbeatIntervalMs = _heartbeatIntervalMs
        }), cancellationToken);

        return true;
    }
}