using Msg = fbs.realtime.SessionInviteUser;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionInviteUserAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}