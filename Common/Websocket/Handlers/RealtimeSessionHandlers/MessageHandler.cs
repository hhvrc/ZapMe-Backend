using fbs.realtime;
using fbs.server;
using System.Collections.Concurrent;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private static readonly ConcurrentDictionary<string, WebSocketInstance> _instances = new();

    private async Task<bool> HandleRealtimeSessionMessageAsync(RealtimeSessionMessage msg, CancellationToken cancellationToken)
    {
        var newMsg = new RealtimeSession
        {
            Body = new RealtimeSessionBody(
                new RealtimeSessionMessage
                {
                    RecepientUserIds = new List<string> { "1" },
                    Message = msg.Message
                }
            )
        };
        var isntances = _instances.ToArray();
        foreach (var instance in isntances)
        {
            await instance.Value.SendMessageAsync(new ServerMessageBody(newMsg), cancellationToken);
        }

        return true;
    }
}