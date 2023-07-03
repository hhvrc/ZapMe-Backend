using fbs.server;
using System.Runtime.CompilerServices;

namespace ZapMe.BusinessLogic.Serialization.Flatbuffers;

public static class UserFriendRequestSerializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task Serialize(Guid senderUserId, Guid receiverUserId, Func<ArraySegment<byte>, Task> messageConsumer)
    {
        var payload = new ServerPayload(new FriendRequestAdded
        {
            SenderUserId = senderUserId.ToString(),
            ReceiverUserId = receiverUserId.ToString()
        });

        return ServerMessageSerializer.Serialize(payload, messageConsumer);
    }
}
