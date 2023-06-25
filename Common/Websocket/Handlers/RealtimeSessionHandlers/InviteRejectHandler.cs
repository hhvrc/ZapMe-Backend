using Msg = fbs.realtime.SessionInviteReject;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionInviteRejectAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}