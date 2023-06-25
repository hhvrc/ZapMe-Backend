using Msg = fbs.realtime.SessionInviteAccept;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionInviteAcceptAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}