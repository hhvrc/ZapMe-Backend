using fbs.server;
using System.Runtime.CompilerServices;

namespace ZapMe.BusinessLogic.Serialization.Flatbuffers;

public static class ServerReadySerializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task Serialize(uint heartbeatIntervalMs, uint ratelimitBytesPerSec, uint ratelimitBytesPerMin, uint ratelimitMessagesPerSec, uint ratelimitMessagesPerMin, Func<ArraySegment<byte>, Task> messageConsumer)
    {
        var payload = new ServerPayload(new ServerReady
        {
            HeartbeatIntervalMs = heartbeatIntervalMs,
            RatelimitBytesPerSec = ratelimitBytesPerSec,
            RatelimitBytesPerMin = ratelimitBytesPerMin,
            RatelimitMessagesPerSec = ratelimitMessagesPerSec,
            RatelimitMessagesPerMin = ratelimitMessagesPerMin,
        });

        return ServerMessageSerializer.Serialize(payload, messageConsumer);
    }
}
