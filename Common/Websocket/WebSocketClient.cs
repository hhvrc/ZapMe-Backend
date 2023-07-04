using fbs.client;
using fbs.server;
using FlatSharp;
using System.Buffers;
using System.Net.WebSockets;
using System.Threading.Channels;
using ZapMe.Constants;
using ZapMe.Helpers;

namespace ZapMe.Websocket;

public sealed partial class WebSocketClient : IDisposable
{
    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }
    public bool IsConnnected => _webSocket.State == WebSocketState.Open && _clientState is _CLIENT_STATE_INITIAL or _CLIENT_STATE_CONNECTED;

    private readonly WebSocket _webSocket;
    private readonly Channel<ServerMessage> _txChannel;
    private readonly SlidingWindow _msgsSecondWindow;
    private readonly SlidingWindow _msgsMinuteWindow;
    private readonly SlidingWindow _bytesSecondWindow;
    private readonly SlidingWindow _bytesMinuteWindow;
    private readonly Timer _heartbeatTimer;

    private const int _CLIENT_STATE_INITIAL = 0;
    private const int _CLIENT_STATE_CONNECTED = 1;
    private const int _CLIENT_STATE_DISCONNECTING = 2;
    private const int _CLIENT_STATE_DISCONNECTED = 3;
    private int _clientState = _CLIENT_STATE_INITIAL;

    private const uint _heartbeatIntervalMs = 20 * 1000; // TODO: make this configurable
    private const uint _heartbeatAllowableSkewMs = 5000; // TODO: make this configurable
    private long _lastHeartbeatTicks = DateTimeOffset.UtcNow.Ticks;
    public long MsUntilTimeout => _heartbeatIntervalMs + _heartbeatAllowableSkewMs - ((DateTimeOffset.UtcNow.Ticks - Interlocked.Read(ref _lastHeartbeatTicks)) / TimeSpan.TicksPerMillisecond);

    public WebSocketClient(Guid userId, Guid sessionId, WebSocket webSocket)
    {
        UserId = userId;
        SessionId = sessionId;
        _webSocket = webSocket;
        _txChannel = Channel.CreateBounded<ServerMessage>(new BoundedChannelOptions(WebsocketConstants.ClientTxChannelCapacity) { FullMode = BoundedChannelFullMode.Wait });
        _msgsSecondWindow = new SlidingWindow(1000, WebsocketConstants.ClientRateLimitMessagesPerSecond);
        _msgsMinuteWindow = new SlidingWindow(60 * 1000, WebsocketConstants.ClientRateLimitMessagesPerMinute);
        _bytesSecondWindow = new SlidingWindow(1000, WebsocketConstants.ClientRateLimitBytesPerSecond);
        _bytesMinuteWindow = new SlidingWindow(60 * 1000, WebsocketConstants.ClientRateLimitBytesPerMinute);
        _heartbeatTimer = new Timer(HeartbeatTimerCallback, this, _heartbeatAllowableSkewMs, _heartbeatAllowableSkewMs); // TODO: this is probably not the best way to do this
    }

    public async Task<bool> SendPayloadAsync(ServerPayload payload, CancellationToken cancellationToken)
    {
        if (!IsConnnected) return false;

        var message = new ServerMessage
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Payload = payload
        };

        await _txChannel.Writer.WriteAsync(message, cancellationToken);

        return true;
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
                    _txChannel.Writer.TryComplete();
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

    private async Task RxTask(CancellationToken cancellationToken)
    {
        while (IsConnnected)
        {
            byte[] bytes = ArrayPool<byte>.Shared.Rent((int)WebsocketConstants.ClientMessageSizeMax);
            try
            {
                WebSocketReceiveResult msg = await _webSocket.ReceiveAsync(bytes, cancellationToken);
                var data = new ArraySegment<byte>(bytes, 0, msg.Count);

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

                if (!_msgsSecondWindow.RequestConforms() || !_msgsMinuteWindow.RequestConforms())
                {
                    await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Request rate limit exceeded!", cancellationToken);
                    break;
                }

                if (data.Count <= 0)
                {
                    await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Payload size invalid!", cancellationToken);
                    break;
                }

                if (!_bytesSecondWindow.RequestConforms((ulong)data.Count) || !_bytesMinuteWindow.RequestConforms((ulong)data.Count))
                {
                    await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Data rate limit exceeded!", cancellationToken);
                    break;
                }

                ClientMessage flatBufferMsg = ClientMessage.Serializer.Parse(new ArraySegmentInputBuffer(data), FlatBufferDeserializationOption.Lazy);

                if (flatBufferMsg is null)
                {
                    await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Message invalid!", cancellationToken);
                    break;
                }

                ClientPayload? payload = flatBufferMsg.Payload;

                if (!payload.HasValue)
                {
                    await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!", cancellationToken);
                    break;
                }

                if (!await RouteClientMessageAsync(payload.Value, cancellationToken))
                {
                    await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!", cancellationToken);
                    break;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
    }

    private async Task TxTask(CancellationToken cancellationToken)
    {
        while (IsConnnected && await _txChannel.Reader.WaitToReadAsync(cancellationToken))
        {
            // We start with 512 bytes, but the buffer will be resized if needed
            byte[] bytes = ArrayPool<byte>.Shared.Rent(512);

            try
            {
                // Read all messages from the channel and send them
                while (_txChannel.Reader.TryRead(out ServerMessage? message) && message is not null)
                {
                    // Get a max estimate of the buffer size needed
                    int bufferSize = ServerMessage.Serializer.GetMaxSize(message);

                    // Resize the buffer if needed
                    if (bufferSize > bytes.Length)
                    {
                        ArrayPool<byte>.Shared.Return(bytes);
                        bytes = ArrayPool<byte>.Shared.Rent(bufferSize);
                    }

                    // Serialize the message
                    int messageSize = ServerMessage.Serializer.Write(new Span<byte>(bytes), message);

                    // Send the message
                    var segment = new ArraySegment<byte>(bytes, 0, messageSize);
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

    private static void HeartbeatTimerCallback(object? state)
    {
        if (state is not WebSocketClient client) return;

        // Disconnect if the client hasn't sent a heartbeat in a while
        if (client.MsUntilTimeout <= 0)
        {
            Task.Run(() => client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Heartbeat timeout", CancellationToken.None));
        }
    }

    public void Dispose()
    {
        // WebSocket was passed in, so we don't own it, do NOT dispose it, only abort it
        try { _webSocket.Abort(); } catch { }

        // Dispose the timer
        try { _heartbeatTimer.Dispose(); } catch { }
    }
}
