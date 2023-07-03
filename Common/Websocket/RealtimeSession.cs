namespace ZapMe.Websocket;

public sealed class RealtimeSession
{
    public Guid SessionId { get; }
    public List<WebSocketUser> Participants { get; }

    public RealtimeSession(Guid sessionId, List<WebSocketUser> participants)
    {
        SessionId = sessionId;
        Participants = participants;
    }
}
