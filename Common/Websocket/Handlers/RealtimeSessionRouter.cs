using fbs.realtime;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionAsync(RealtimeSession msg, CancellationToken cancellationToken)
    {
        if (!msg.Body.HasValue) return Task.FromResult(false);

        RealtimeSessionBody value = msg.Body.Value;

        return value.Kind switch
        {
            RealtimeSessionBody.ItemKind.create => HandleRealtimeSessionCreateAsync(value.create, cancellationToken),
            RealtimeSessionBody.ItemKind.invite_accept => HandleRealtimeSessionInviteAcceptAsync(value.invite_accept, cancellationToken),
            RealtimeSessionBody.ItemKind.invite_reject => HandleRealtimeSessionInviteRejectAsync(value.invite_reject, cancellationToken),
            RealtimeSessionBody.ItemKind.ice_candidate_discovered => HandleRealtimeSessionIceCandidateDiscoveredAsync(value.ice_candidate_discovered, cancellationToken),
            RealtimeSessionBody.ItemKind.rejoin => HandleRealtimeSessionRejoinAsync(value.rejoin, cancellationToken),
            RealtimeSessionBody.ItemKind.leave => HandleRealtimeSessionLeaveAsync(value.leave, cancellationToken),
            RealtimeSessionBody.ItemKind.invite_user => HandleRealtimeSessionInviteUserAsync(value.invite_user, cancellationToken),
            RealtimeSessionBody.ItemKind.message => HandleRealtimeSessionMessageAsync(value.message, cancellationToken),

            // Client is not allowed to raise server events
            RealtimeSessionBody.ItemKind.event_joined => Task.FromResult(false), 
            RealtimeSessionBody.ItemKind.event_invited => Task.FromResult(false),
            RealtimeSessionBody.ItemKind.event_user_joined => Task.FromResult(false),
            RealtimeSessionBody.ItemKind.event_user_left => Task.FromResult(false),
            RealtimeSessionBody.ItemKind.event_user_ice_candidate_discovered => Task.FromResult(false),
            RealtimeSessionBody.ItemKind.event_user_message => Task.FromResult(false),

            // Invalid message type
            RealtimeSessionBody.ItemKind.NONE => Task.FromResult(false),
            _ => Task.FromResult(false),
        };
    }
}