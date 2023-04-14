namespace ZapMe.Websocket.Models;

public enum ServerMessageType
{
    Hello,
    Heartbeat,
    HeartbeatAck,
    Ready,

    GlobalMessage,
}
