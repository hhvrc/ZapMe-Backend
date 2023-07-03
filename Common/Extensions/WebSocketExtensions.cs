using System.Buffers;
using System.Text;
using ZapMe.Constants;

namespace System.Net.WebSockets;

public static class WebSocketExtensions
{
    /// <summary>
    /// Reads a single message from the WebSocket, using the specified handler to process the message.
    /// <para>This uses a buffer from the shared <see cref="ArrayPool{T}"/> to minimize allocations.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="webSocket"></param>
    /// <param name="handler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<T?> ReceiveAsync<T>(this WebSocket webSocket, Func<WebSocketMessageType, ArraySegment<byte>, CancellationToken, Task<T>> handler, CancellationToken cancellationToken)
    {
        byte[] bytes = ArrayPool<byte>.Shared.Rent((int)WebsocketConstants.ClientMessageSizeMax);
        try
        {
            WebSocketReceiveResult msg = await webSocket.ReceiveAsync(bytes, cancellationToken);
            return await handler(msg.MessageType, new ArraySegment<byte>(bytes, 0, msg.Count), cancellationToken);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    /// <summary>
    /// Reads a single string message from the WebSocket.
    /// </summary>
    /// <param name="webSocket"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The string message, or null if the message was not a string.</returns>
    public static Task<string?> ReceiveStringAsync(this WebSocket webSocket, CancellationToken cancellationToken)
    {
        static Task<string?> handler(WebSocketMessageType type, ArraySegment<byte> data, CancellationToken cancellationToken)
        {
            return Task.FromResult(type == WebSocketMessageType.Text ? Encoding.UTF8.GetString(data) : null);
        }
        return ReceiveAsync(webSocket, handler, cancellationToken);
    }
}
