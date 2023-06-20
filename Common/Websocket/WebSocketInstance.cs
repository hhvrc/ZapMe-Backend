using client.fbs;
using FlatSharp;
using OneOf;
using server.fbs;
using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Websocket;

public sealed partial class WebSocketInstance : IDisposable
{
    private static async Task<T?> ReceiveAsync<T>(WebSocket webSocket, Func<WebSocketMessageType, ArraySegment<byte>, CancellationToken, Task<T>> handler, CancellationToken cancellationToken)
    {
        byte[] bytes = ArrayPool<byte>.Shared.Rent(WebsocketConstants.ClientMessageSizeMax);
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

    public static async Task<OneOf<WebSocketInstance, ErrorDetails>> CreateAsync(WebSocketManager wsManager, IJwtAuthenticationManager authenticationManager, ILogger<WebSocketInstance> logger, CancellationToken cancellationToken)
    {
        WebSocket? ws = await wsManager.AcceptWebSocketAsync();
        if (ws is null) return HttpErrors.InternalServerError; // TODO: Better error

        try
        {
            // Receive JWT from client
            string? token = await ReceiveStringAsync(ws, cancellationToken);
            if (token is null)
            {
                logger.LogError("Failed to authenticate websocket connection, received invalid message from client");
                ws.Dispose();
                return HttpErrors.InternalServerError; // TODO: Better error
            }

            // Validate JWT
            var authenticationResult = await authenticationManager.AuthenticateJwtTokenAsync(token, cancellationToken);
            if (authenticationResult.TryPickT1(out ErrorDetails errorDetails, out SessionEntity session))
            {
                logger.LogError("Failed to authenticate websocket connection, provided JWT was invalid");
                ws.Dispose();
                return errorDetails;
            }

            // Success, create websocket instance
            var instance = new WebSocketInstance(session.UserId, session.Id, ws, logger);

            // Send hello message to inform client that everything is A-OK
            var hello = new ServerHello { SessionId = session.Id.ToString(), UserId = session.UserId.ToString() };
            await instance.SendMessageAsync(new ServerMessageBody(hello), cancellationToken);

            // Finally, return instance
            return instance;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to authenticate websocket connection, exception: {Exception}", ex);
            ws.Dispose();
            return HttpErrors.InternalServerError;
        }
    }

    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }

    private readonly WebSocket _webSocket;
    private readonly SlidingWindow _msgsSecondWindow;
    private readonly SlidingWindow _msgsMinuteWindow;
    private readonly SlidingWindow _bytesSecondWindow;
    private readonly SlidingWindow _bytesMinuteWindow;
    private readonly ILogger<WebSocketInstance> _logger;

    private readonly int _heartbeatIntervalMs = 30 * 1000;
    private DateTime _lastHeartbeat = DateTime.UtcNow;
    private int MsUntilTimeout => _heartbeatIntervalMs + 2000 - (int)(DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds;

    private WebSocketInstance(Guid userId, Guid sessionId, WebSocket webSocket, ILogger<WebSocketInstance> logger)
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
            return (WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!");
        }

        ClientMessageBody? body = flatBufferMsg.Message;

        if (!body.HasValue)
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!");
        }

        if (!await RouteClientMessageAsync(body.Value, cancellationToken))
        {
            return (WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!");
        }

        return null;
    }

    public async Task SendMessageAsync(ServerMessageBody messageBody, CancellationToken cancellationToken)
    {
        if (_webSocket.State != WebSocketState.Open) return;

        ServerMessage message = new ServerMessage
        {
            Timestamp = DateTime.UtcNow.Ticks,
            Message = messageBody,
        };

        byte[] bytes = ArrayPool<byte>.Shared.Rent(ServerMessage.Serializer.GetMaxSize(message));
        try
        {
            int nWritten = ServerMessage.Serializer.Write(bytes, message);
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

    public void Dispose()
    {
        if (_webSocket.State is not WebSocketState.Closed or WebSocketState.Aborted)
        {
            try { _webSocket.Abort(); } catch { }
        }
        try { _webSocket.Dispose(); } catch { }
    }
}
