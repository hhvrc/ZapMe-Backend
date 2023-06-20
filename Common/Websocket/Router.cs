using client.fbs;
using server.fbs;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> RouteClientMessageAsync(ClientMessageBody message, CancellationToken cancellationToken)
    {
        return message.Kind switch
        {
            ClientMessageBody.ItemKind.heartbeat => HandleHeartbeatAsync(message.heartbeat, cancellationToken),
            ClientMessageBody.ItemKind.NONE => Task.FromResult(false),
            _ => Task.FromResult(false),
        };
    }
}