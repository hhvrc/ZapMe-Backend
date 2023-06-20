using client.fbs;
using server.fbs;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private async Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        _lastHeartbeat = DateTime.UtcNow;
        ServerHeartbeat response = new ServerHeartbeat
        {
            Timestamp = DateTime.UtcNow.Ticks
        };
        await SendMessageAsync(new ServerMessageBody(response), cancellationToken);

        return true;
    }
}