using FlatSharp;
using System.Buffers;
using System.Net.WebSockets;
using System.Threading.Channels;
using ZapMe.Constants;

namespace ZapMe.Websocket;

public abstract class FlatbufferWebSocketBase<TRX, TTX>
    where TRX : class, IFlatBufferSerializable<TRX>, IFlatBufferSerializable
    where TTX : class, IFlatBufferSerializable<TTX>, IFlatBufferSerializable
{
    public bool IsConnnected => _webSocket.State == WebSocketState.Open && _clientState is _CLIENT_STATE_INITIAL or _CLIENT_STATE_CONNECTED;

    private readonly WebSocket _webSocket;
    private readonly ISerializer<TRX> _rxSerializer;
    private readonly ISerializer<TTX> _txSerializer;

    protected Channel<TTX> TxChannel { get; init; }

    private const int _CLIENT_STATE_INITIAL = 0;
    private const int _CLIENT_STATE_CONNECTED = 1;
    private const int _CLIENT_STATE_DISCONNECTING = 2;
    private const int _CLIENT_STATE_DISCONNECTED = 3;
    private int _clientState = _CLIENT_STATE_INITIAL;

    public FlatbufferWebSocketBase(WebSocket webSocket, ISerializer<TRX> rxSerializer, ISerializer<TTX> txSerializer)
    {
        _webSocket = webSocket;
        _rxSerializer = rxSerializer;
        _txSerializer = txSerializer;
        TxChannel = Channel.CreateBounded<TTX>(new BoundedChannelOptions(WebsocketConstants.ClientTxChannelCapacity) { FullMode = BoundedChannelFullMode.Wait });
    }

    public async Task RunWebSocketAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.CompareExchange(ref _clientState, _CLIENT_STATE_CONNECTED, _CLIENT_STATE_INITIAL) != _CLIENT_STATE_INITIAL)
        {
            throw new InvalidOperationException("Client is already running!");
        }

        Task txTask = TxTask(cancellationToken);
        Task rxTask = RxTask(cancellationToken);

        await Task.WhenAny(rxTask, txTask);

        await CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", cancellationToken);

        await Task.WhenAll(rxTask, txTask);
    }

    public async Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string reason = "ByeBye ❤️", CancellationToken cs = default)
    {
        if (_webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            if (Interlocked.CompareExchange(ref _clientState, _CLIENT_STATE_DISCONNECTING, _CLIENT_STATE_CONNECTED) == _CLIENT_STATE_CONNECTED)
            {
                try
                {
                    TxChannel.Writer.TryComplete();
                    await _webSocket.CloseAsync(closeStatus, reason, cs);
                }
                catch
                {
                    _webSocket.Abort();
                }
                finally
                {
                    _clientState = _CLIENT_STATE_DISCONNECTED;
                }
            }
        }
    }

    protected virtual Task<bool> ValidateMessage(ArraySegment<byte> message, CancellationToken cancellationToken) => Task.FromResult(true);
    protected abstract Task HandleMessageAsync(TRX message, CancellationToken cancellationToken);

    private async Task RxTask(CancellationToken cancellationToken)
    {
        while (IsConnnected)
        {
            byte[] bytes = ArrayPool<byte>.Shared.Rent((int)WebsocketConstants.ClientMessageSizeMax);
            try
            {
                WebSocketReceiveResult msg = await _webSocket.ReceiveAsync(bytes, cancellationToken);

                if (msg.MessageType == WebSocketMessageType.Close)
                {
                    await CloseAsync(WebSocketCloseStatus.NormalClosure, "ByeBye ❤️", cancellationToken);
                    break;
                }

                if (msg.MessageType != WebSocketMessageType.Binary)
                {
                    await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Only binary messages are supported!", cancellationToken);
                    break;
                }

                ArraySegment<byte> data = new ArraySegment<byte>(bytes, 0, msg.Count);

                if (!await ValidateMessage(data, cancellationToken))
                {
                    await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Rate/Size limit(s) exceeded!", cancellationToken);
                    break;
                }

                TRX message = _rxSerializer.Parse(data);

                if (message is null)
                {
                    await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid message!", cancellationToken);
                    break;
                }

                await HandleMessageAsync(message, cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
    }

    private async Task TxTask(CancellationToken cancellationToken)
    {
        while (IsConnnected && await TxChannel.Reader.WaitToReadAsync(cancellationToken))
        {
            // We start with 512 bytes, but the buffer will be resized if needed
            byte[] bytes = ArrayPool<byte>.Shared.Rent(512);

            try
            {
                // Read all messages from the channel and send them
                while (TxChannel.Reader.TryRead(out TTX? message) && message is not null)
                {
                    // Get a max estimate of the buffer size needed
                    int bufferSize = _txSerializer.GetMaxSize(message);

                    // Resize the buffer if needed
                    if (bufferSize > bytes.Length)
                    {
                        ArrayPool<byte>.Shared.Return(bytes);
                        bytes = ArrayPool<byte>.Shared.Rent(bufferSize);
                    }

                    // Serialize the message
                    int messageSize = _txSerializer.Write(new Span<byte>(bytes), message);

                    // Send the message
                    ArraySegment<byte> segment = new ArraySegment<byte>(bytes, 0, messageSize);
                    await _webSocket.SendAsync(segment, WebSocketMessageType.Binary, true, cancellationToken);
                }
            }
            finally
            {
                // Return the buffer to the pool
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
    }
}
