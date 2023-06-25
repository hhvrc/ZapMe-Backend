using SessionMessage = fbs.realtime.Message;
using SessionPayload = fbs.realtime.Payload;
using SessionPayloadType = fbs.realtime.Payload.ItemKind;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> HandleRealtimeSessionAsync(SessionMessage message, CancellationToken cancellationToken)
    {
        if (!message.Payload.HasValue) return Task.FromResult(false);

        SessionPayload payload = message.Payload.Value;

        return payload.Kind switch
        {
            SessionPayloadType.create => HandleRealtimeSessionCreateAsync(payload.create, cancellationToken),
            SessionPayloadType.invite_accept => HandleRealtimeSessionInviteAcceptAsync(payload.invite_accept, cancellationToken),
            SessionPayloadType.invite_reject => HandleRealtimeSessionInviteRejectAsync(payload.invite_reject, cancellationToken),
            SessionPayloadType.ice_candidate_discovered => HandleRealtimeSessionIceCandidateDiscoveredAsync(payload.ice_candidate_discovered, cancellationToken),
            SessionPayloadType.rejoin => HandleRealtimeSessionRejoinAsync(payload.rejoin, cancellationToken),
            SessionPayloadType.leave => HandleRealtimeSessionLeaveAsync(payload.leave, cancellationToken),
            SessionPayloadType.invite_user => HandleRealtimeSessionInviteUserAsync(payload.invite_user, cancellationToken),
            SessionPayloadType.message => HandleRealtimeSessionMessageAsync(payload.message, cancellationToken),

            // Client is not allowed to raise server events
            SessionPayloadType.event_joined => Task.FromResult(false), 
            SessionPayloadType.event_invited => Task.FromResult(false),
            SessionPayloadType.event_user_joined => Task.FromResult(false),
            SessionPayloadType.event_user_left => Task.FromResult(false),
            SessionPayloadType.event_user_ice_candidate_discovered => Task.FromResult(false),
            SessionPayloadType.event_user_message => Task.FromResult(false),

            // Invalid message type
            SessionPayloadType.NONE => Task.FromResult(false),
            _ => Task.FromResult(false),
        };
    }
}