using fbs.server;
using FlatSharp;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ZapMe.BusinessLogic.Serialization.Flatbuffers;

public static class ServerMessageSerializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task Serialize(ServerPayload payload, Func<ArraySegment<byte>, Task> messageConsumer)
    {
        ServerMessage message = new ServerMessage
        {
            Timestamp = DateTime.UtcNow.Ticks,
            Payload = payload,
        };

        ISerializer<ServerMessage> serializer = ServerMessage.Serializer;

        int size = serializer.GetMaxSize(message);
        byte[] array = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            int length = serializer.Write(array, message);
            return messageConsumer(new ArraySegment<byte>(array, 0, length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
