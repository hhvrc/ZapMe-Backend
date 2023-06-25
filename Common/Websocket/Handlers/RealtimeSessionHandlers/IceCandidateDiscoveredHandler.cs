using Msg = fbs.realtime.SessionIceCandidateDiscovered;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionIceCandidateDiscoveredAsync(Msg msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}