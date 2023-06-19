using fbs.zapme.realtime;
using FlatSharp;
using System.Buffers;
using System.Net.WebSockets;
using System.Security.Claims;

namespace ZapMe.Websocket;

public sealed class WebSocketInstance : IDisposable
{
    public static async Task<WebSocketInstance?> CreateAsync(WebSocketManager wsManager, ClaimsPrincipal user, ILogger<WebSocketInstance> logger)
    {
        WebSocket? ws = await wsManager.AcceptWebSocketAsync();
        if (ws is null) return null;

        return new WebSocketInstance(user.GetUserId(), user.GetSessionId(), ws, logger);
    }

    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }

    private readonly WebSocket _webSocket;
    private readonly ILogger<WebSocketInstance> _logger;

    private readonly int _heartbeatIntervalMs = 30 * 1000;
    private DateTime _lastHeartbeat = DateTime.UtcNow;
    private int MsUntilTimeout => _heartbeatIntervalMs + 2000 - (int)(DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds;

    private WebSocketInstance(Guid userId, Guid sessionId, WebSocket webSocket, ILogger<WebSocketInstance> logger)
    {
        UserId = userId;
        SessionId = sessionId;
        _webSocket = webSocket;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        // TODO: mark person as online

        try
        {
            await RunWebSocketAsync(cancellationToken);
        }
        finally
        {
            // TODO: mark person as offline
        }
    }

    private async Task RunWebSocketAsync(CancellationToken cancellationToken)
    {
        WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;
        string closeMessage = "ByeBye ❤️";
        while (_webSocket.State == WebSocketState.Open)
        {
            using CancellationTokenSource timeoutCts = new CancellationTokenSource(MsUntilTimeout);
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            cancellationToken = linkedCts.Token;

            byte[] bytes = ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                WebSocketReceiveResult msg = await _webSocket.ReceiveAsync(bytes, cancellationToken);

                if (msg.Count <= 0)
                {
                    closeStatus = WebSocketCloseStatus.InvalidPayloadData;
                    closeMessage = "Payload size invalid!";
                    break;
                }
                if (msg.MessageType == WebSocketMessageType.Close)
                {
                    closeStatus = WebSocketCloseStatus.NormalClosure;
                    closeMessage = "ByeBye ❤️";
                    break;
                }
                if (msg.MessageType != WebSocketMessageType.Binary)
                {
                    closeStatus = WebSocketCloseStatus.InvalidPayloadData;
                    closeMessage = "Only binary messages are supported!";
                    break;
                }

                // TODO: check if message is actually written to all of the buffer, if it then isnt finished, then close connection like this:
                if (!msg.EndOfMessage)
                {
                    closeStatus = WebSocketCloseStatus.MessageTooBig;
                    closeMessage = "Payload size too big!";
                    break;
                }

                ClientMessage flatBufferMsg = ClientMessage.Serializer.Parse(new ArraySegmentInputBuffer(new ArraySegment<byte>(bytes, 0, msg.Count)), FlatBufferDeserializationOption.Lazy);

                if (flatBufferMsg is null)
                {
                    closeStatus = WebSocketCloseStatus.InvalidPayloadData;
                    closeMessage = "Payload invalid!";
                    break;
                }

                var body = flatBufferMsg.Message;

                if (!body.HasValue)
                {
                    closeStatus = WebSocketCloseStatus.InvalidPayloadData;
                    closeMessage = "Payload invalid!";
                    break;
                }

                if (!await HandleMessageAsync(body.Value, cancellationToken))
                {
                    closeStatus = WebSocketCloseStatus.InvalidPayloadData;
                    closeMessage = "Payload invalid!";
                    break;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
        await CloseAsync(closeStatus, closeMessage, cancellationToken);
    }

    private async Task<bool> HandleMessageAsync(ClientMessageBody message, CancellationToken cs)
    {
        switch (message.Kind)
        {
            case ClientMessageBody.ItemKind.heartbeat:
                _lastHeartbeat = DateTime.UtcNow;
                ServerHeartbeat heartbeat = new ServerHeartbeat
                {
                    Timestamp = DateTime.UtcNow.Ticks
                };
                await SendAsync(new ServerMessageBody(heartbeat), cs);
                break;
            case ClientMessageBody.ItemKind.NONE:
            default:
                return false;
        }

        return true;
    }

    public async Task SendAsync(ServerMessageBody data, CancellationToken cs)
    {
        if (_webSocket.State != WebSocketState.Open) return;

        var message = new ServerMessage
        {
            Timestamp = DateTime.UtcNow.Ticks,
            Message = data,
        };

        byte[] bytes = ArrayPool<byte>.Shared.Rent(ServerMessage.Serializer.GetMaxSize(message));
        try
        {
            var buffer = new ArraySegment<byte>(bytes);
            ServerMessage.Serializer.Write(buffer, message);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, endOfMessage: true, cs);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    public async Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string reason = "ByeBye ❤️", CancellationToken cs = default)
    {
        try
        {
            await _webSocket.CloseAsync(closeStatus, reason, cs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while closing websocket: {message}", ex.Message);
        }
    }

    public void Dispose()
    {
        _webSocket.Dispose();
    }
}
