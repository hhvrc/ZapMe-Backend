using fbs.client;
using ZapMe.BusinessLogic.Serialization.Flatbuffers;

namespace ZapMe.Websocket;

partial class WebSocketClient
{
    private async Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        _lastHeartbeat = DateTime.UtcNow;

        await ServerHeartbeatSerializer.Serialize(_heartbeatIntervalMs, (bytes) => SendMessageAsync(bytes, cancellationToken));

        return true;
    }
}