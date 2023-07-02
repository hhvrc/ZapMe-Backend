using fbs.client;
using fbs.server;
using FlatSharp;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums.Errors;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Websocket;

public sealed partial class WebSocketClient
{
    private static async Task<T?> ReceiveAsync<T>(WebSocket webSocket, Func<WebSocketMessageType, ArraySegment<byte>, CancellationToken, Task<T>> handler, CancellationToken cancellationToken)
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

    private static async Task<string?> ReceiveStringAsync(WebSocket webSocket, CancellationToken cancellationToken) =>
        await ReceiveAsync(webSocket, static (type, data, _) => Task.FromResult(type == WebSocketMessageType.Text ? Encoding.UTF8.GetString(data) : null), cancellationToken);

    public static async Task<OneOf<WebSocketClient, CreateWebSocketError>> CreateAsync(WebSocket websocket, IJwtAuthenticationManager authenticationManager, ILogger<WebSocketClient> logger, CancellationToken cancellationToken)
    {
        // Receive JWT from client
        string? token = await ReceiveStringAsync(websocket, cancellationToken);
        if (token is null)
        {
            logger.LogError("Failed to authenticate websocket connection, received invalid message from client");
            websocket.Dispose();
            return CreateWebSocketError.InvalidClientMessage;
        }

        // Validate JWT
        var authenticationResult = await authenticationManager.AuthenticateJwtTokenAsync(token, cancellationToken);
        if (authenticationResult.TryPickT1(out JwtAuthenticationError authenticationError, out SessionEntity session))
        {
            logger.LogError("Failed to authenticate websocket connection, provided JWT was invalid");
            websocket.Dispose();

            return JwtAuthenticationErrorMapper.MapToCreateWebSocketError(authenticationError);
        }

        // Success, create websocket instance
        var instance = new WebSocketClient(session.UserId, session.Id, websocket, logger);

        // Send hello message to inform client that everything is A-OK
        ServerReady ready = new()
        {
            HeartbeatIntervalMs = 10 * 1000, // 10 seconds TODO: make this configurable
            RatelimitBytesPerSec = WebsocketConstants.ClientRateLimitBytesPerSecond,
            RatelimitBytesPerMin = WebsocketConstants.ClientRateLimitBytesPerMinute,
            RatelimitMessagesPerSec = WebsocketConstants.ClientRateLimitMessagesPerSecond,
            RatelimitMessagesPerMin = WebsocketConstants.ClientRateLimitMessagesPerMinute,
        };
        await instance.SendMessageAsync(new ServerPayload(ready), cancellationToken);

        // Finally, return instance
        return instance;
    }

    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }

    private readonly WebSocket _webSocket;
    private readonly SlidingWindow _msgsSecondWindow;
    private readonly SlidingWindow _msgsMinuteWindow;
    private readonly SlidingWindow _bytesSecondWindow;
    private readonly SlidingWindow _bytesMinuteWindow;
    private readonly ILogger<WebSocketClient> _logger;

    private readonly uint _heartbeatIntervalMs = 30 * 1000; // TODO: make this configurable
    private DateTime _lastHeartbeat = DateTime.UtcNow;
    private int MsUntilTimeout => (int)_heartbeatIntervalMs + 2000 - (int)(DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds; // TODO: make the clock skew (2000) configurable

    private WebSocketClient(Guid userId, Guid sessionId, WebSocket webSocket, ILogger<WebSocketClient> logger)
    {
        UserId = userId;
        SessionId = sessionId;
        _webSocket = webSocket;
        _msgsSecondWindow = new SlidingWindow(1000, WebsocketConstants.ClientRateLimitMessagesPerSecond);
        _msgsMinuteWindow = new SlidingWindow(60 * 1000, WebsocketConstants.ClientRateLimitMessagesPerMinute);
        _bytesSecondWindow = new SlidingWindow(1000, WebsocketConstants.ClientRateLimitBytesPerSecond);
        _bytesMinuteWindow = new SlidingWindow(60 * 1000, WebsocketConstants.ClientRateLimitBytesPerMinute);
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_webSocket.State != WebSocketState.Open) return;

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


            var result = await ReceiveAsync(_webSocket, HandleClientMessageAsync, cancellationToken);
            if (result.HasValue)
            {
                (closeStatus, closeMessage) = result.Value;
                break;
            }
        }

        await CloseAsync(closeStatus, closeMessage, cancellationToken);
    }

    private async Task<(WebSocketCloseStatus, string)?> HandleClientMessageAsync(WebSocketMessageType type, ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        if (!_msgsSecondWindow.RequestConforms() || !_msgsMinuteWindow.RequestConforms())
        {
            return (WebSocketCloseStatus.PolicyViolation, "Rate limit exceeded!");
        }

        if (type == WebSocketMessageType.Close)
        {
            return (WebSocketCloseStatus.NormalClosure, "ByeBye ❤️");
        }

        if (data.Count <= 0)
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Payload size invalid!");
        }

        if (!_bytesSecondWindow.RequestConforms((ulong)data.Count) || !_bytesMinuteWindow.RequestConforms((ulong)data.Count))
        {
            return (WebSocketCloseStatus.PolicyViolation, "Rate limit exceeded!");
        }

        if (type != WebSocketMessageType.Binary)
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Only binary messages are supported!");
        }

        ClientMessage flatBufferMsg = ClientMessage.Serializer.Parse(new ArraySegmentInputBuffer(data), FlatBufferDeserializationOption.Lazy);

        if (flatBufferMsg is null)
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Message invalid!");
        }

        ClientPayload? payload = flatBufferMsg.Payload;

        if (!payload.HasValue)
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!");
        }

        if (!await RouteClientMessageAsync(payload.Value, cancellationToken))
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!");
        }

        return null;
    }

    public async Task SendMessageAsync(ServerPayload payload, CancellationToken cancellationToken)
    {
        if (_webSocket.State != WebSocketState.Open) return;

        ServerMessage message = new ServerMessage
        {
            Timestamp = DateTime.UtcNow.Ticks,
            Payload = payload,
        };

        ISerializer<ServerMessage> serializer = ServerMessage.Serializer;

        byte[] bytes = ArrayPool<byte>.Shared.Rent(serializer.GetMaxSize(message));
        try
        {
            int nWritten = serializer.Write(bytes, message);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, nWritten), WebSocketMessageType.Binary, endOfMessage: true, cancellationToken);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    public async Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string reason = "ByeBye ❤️", CancellationToken cs = default)
    {
        if (_webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            try
            {
                await _webSocket.CloseAsync(closeStatus, reason, cs);
            }
            catch
            {
                _webSocket.Abort();
            }
        }
    }
}
