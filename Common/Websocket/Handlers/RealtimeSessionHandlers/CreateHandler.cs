using Msg = fbs.realtime.SessionCreate;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionCreateAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}