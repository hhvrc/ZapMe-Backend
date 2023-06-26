using fbs.client;
using PayloadType = fbs.client.ClientPayload.ItemKind;

namespace ZapMe.Websocket;

partial class WebSocketInstance
{
    private Task<bool> RouteClientMessageAsync(ClientPayload message, CancellationToken cancellationToken)
    {
        return message.Kind switch
        {
            PayloadType.heartbeat => HandleHeartbeatAsync(message.heartbeat, cancellationToken),

            // Sessions
            PayloadType.session_join => HandleSessionJoinAsync(message.session_join, cancellationToken),
            PayloadType.session_leave => HandleSessionLeaveAsync(message.session_leave, cancellationToken),
            PayloadType.session_rejoin => HandleSessionRejoinAsync(message.session_rejoin, cancellationToken),
            PayloadType.session_invite => HandleSessionInviteAsync(message.session_invite, cancellationToken),
            PayloadType.session_ice_candidate_discovered => HandleSessionIceCandidateDiscoveredAsync(message.session_ice_candidate_discovered, cancellationToken),

            PayloadType.NONE => Task.FromResult(false),
            _ => Task.FromResult(false),
        };
    }
}