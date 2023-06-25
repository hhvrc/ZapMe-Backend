using Msg = fbs.realtime.SessionLeave;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionLeaveAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}