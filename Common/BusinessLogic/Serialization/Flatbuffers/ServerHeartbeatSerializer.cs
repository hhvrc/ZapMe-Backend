using fbs.server;
using System.Runtime.CompilerServices;

namespace ZapMe.BusinessLogic.Serialization.Flatbuffers;

public static class ServerHeartbeatSerializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task Serialize(uint heartbeatIntervalMs, Func<ArraySegment<byte>, Task> messageConsumer)
    {
        var payload = new ServerPayload(new ServerHeartbeat
        {
            HeartbeatIntervalMs = heartbeatIntervalMs
        });

        return ServerMessageSerializer.Serialize(payload, messageConsumer);
    }
}
