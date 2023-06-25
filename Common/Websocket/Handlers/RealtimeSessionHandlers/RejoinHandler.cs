using Msg = fbs.realtime.SessionRejoin;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionRejoinAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}