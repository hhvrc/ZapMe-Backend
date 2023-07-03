using fbs.client;
using FlatSharp;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using ZapMe.Constants;
using ZapMe.Helpers;

namespace ZapMe.Websocket;

public sealed partial class WebSocketClient
{
    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }

    private readonly WebSocket _webSocket;
    private readonly SlidingWindow _msgsSecondWindow;
    private readonly SlidingWindow _msgsMinuteWindow;
    private readonly SlidingWindow _bytesSecondWindow;
    private readonly SlidingWindow _bytesMinuteWindow;
    private readonly ILogger<WebSocketClient> _logger;

    private readonly uint _heartbeatIntervalMs = 20 * 1000; // TODO: make this configurable
    private DateTime _lastHeartbeat = DateTime.UtcNow;
    private int MsUntilTimeout => (int)_heartbeatIntervalMs + 2000 - (int)(DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds; // TODO: make the clock skew (2000) configurable

    public WebSocketClient(Guid userId, Guid sessionId, WebSocket webSocket, ILogger<WebSocketClient> logger)
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

    public async Task RunWebSocketAsync(CancellationToken cancellationToken)
    {
        WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;
        string closeMessage = "ByeBye ❤️";
        while (_webSocket.State == WebSocketState.Open)
        {
            using CancellationTokenSource timeoutCts = new CancellationTokenSource(MsUntilTimeout);
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            cancellationToken = linkedCts.Token;


            var result = await _webSocket.ReceiveAsync(HandleClientMessageAsync, cancellationToken);
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

    public async Task SendMessageAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken)
    {
        if (_webSocket.State != WebSocketState.Open) return;

        await _webSocket.SendAsync(bytes, WebSocketMessageType.Binary, endOfMessage: true, cancellationToken);
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
