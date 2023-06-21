using fbs.realtime;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionAsync(RealtimeSession realtimeSession, CancellationToken cancellationToken)
    {
        if (!realtimeSession.Body.HasValue) return Task.FromResult(false);

        return realtimeSession.Body.Value.Kind switch
        {
            RealtimeSessionBody.ItemKind.create => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.invite_accept => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.invite_reject => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.ice_candidate_discovered => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.rejoin => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.leave => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.invite_user => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.message => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.event_joined => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.event_invited => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.event_user_joined => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.event_user_left => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.event_user_ice_candidate_discovered => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.event_user_message => throw new NotImplementedException(),
            RealtimeSessionBody.ItemKind.NONE => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }
}