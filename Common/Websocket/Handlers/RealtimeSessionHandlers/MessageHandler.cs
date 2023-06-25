using Msg = fbs.realtime.SessionMessage;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionMessageAsync(Msg sessionMsg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}