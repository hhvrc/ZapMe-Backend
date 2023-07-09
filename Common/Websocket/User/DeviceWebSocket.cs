using fbs.client;
using fbs.common;
using fbs.server;
using System.Net.WebSockets;
using ZapMe.Constants;
using ZapMe.Helpers;
using PayloadType = fbs.client.ClientDevicePayload.ItemKind;

namespace ZapMe.Websocket;

public sealed class DeviceWebSocket : FlatbufferWebSocketBase<ClientDeviceMessage, ServerMessage>, IDisposable
{
    public Guid UserId { get; init; }
    public Guid DeviceId { get; init; }

    private readonly SlidingWindow _msgsSecondWindow;
    private readonly SlidingWindow _msgsMinuteWindow;
    private readonly SlidingWindow _bytesSecondWindow;
    private readonly SlidingWindow _bytesMinuteWindow;
    private readonly Timer _heartbeatTimer;

    private const uint _heartbeatIntervalMs = 20 * 1000; // TODO: make this configurable
    private const uint _heartbeatAllowableSkewMs = 5000; // TODO: make this configurable
    private long _lastHeartbeatTicks = DateTimeOffset.UtcNow.Ticks;
    public long MsUntilTimeout => _heartbeatIntervalMs + _heartbeatAllowableSkewMs - ((DateTimeOffset.UtcNow.Ticks - Interlocked.Read(ref _lastHeartbeatTicks)) / TimeSpan.TicksPerMillisecond);

    public DeviceWebSocket(Guid userId, Guid deviceId, WebSocket webSocket) : base(webSocket, ClientDeviceMessage.Serializer, ServerMessage.Serializer)
    {
        UserId = userId;
        DeviceId = deviceId;
        _msgsSecondWindow = new SlidingWindow(1000, WebsocketConstants.ClientRateLimitMessagesPerSecond);
        _msgsMinuteWindow = new SlidingWindow(60 * 1000, WebsocketConstants.ClientRateLimitMessagesPerMinute);
        _bytesSecondWindow = new SlidingWindow(1000, WebsocketConstants.ClientRateLimitBytesPerSecond);
        _bytesMinuteWindow = new SlidingWindow(60 * 1000, WebsocketConstants.ClientRateLimitBytesPerMinute);
        _heartbeatTimer = new Timer(HeartbeatTimerCallback, this, _heartbeatAllowableSkewMs, _heartbeatAllowableSkewMs); // TODO: this is probably not the best way to do this
    }

    public ValueTask SendPayloadAsync(ServerPayload payload, CancellationToken cancellationToken)
    {
        return TxChannel.Writer.WriteAsync(new ServerMessage
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Payload = payload
        }, cancellationToken);
    }

    protected override async Task<bool> ValidateMessage(ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        if (!_msgsSecondWindow.RequestConforms() || !_msgsMinuteWindow.RequestConforms())
        {
            await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Request rate limit exceeded!", cancellationToken);
            return false;
        }

        if (!_bytesSecondWindow.RequestConforms((ulong)data.Count) || !_bytesMinuteWindow.RequestConforms((ulong)data.Count))
        {
            await CloseAsync(WebSocketCloseStatus.PolicyViolation, "Data rate limit exceeded!", cancellationToken);
            return false;
        }

        return true;
    }

    protected override async Task HandleMessageAsync(ClientDeviceMessage message, CancellationToken cancellationToken)
    {
        if (!message.Payload.HasValue)
        {
            await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!", cancellationToken);
            return;
        }

        ClientDevicePayload payload = message.Payload.Value;

        // If this switch is not returned from, the connection will be closed
        switch (payload.Kind)
        {
            case PayloadType.heartbeat:
                await HandleHeartbeatAsync(payload.heartbeat, cancellationToken);
                return;
            case PayloadType.NONE:
            default:
                break;
        };

        await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!", cancellationToken);
    }

    private async Task<bool> HandleHeartbeatAsync(ClientHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        Interlocked.Exchange(ref _lastHeartbeatTicks, DateTimeOffset.UtcNow.Ticks);

        await SendPayloadAsync(new ServerPayload(new ServerHeartbeat
        {
            HeartbeatIntervalMs = _heartbeatIntervalMs
        }), cancellationToken);

        return true;
    }

    private static void HeartbeatTimerCallback(object? state)
    {
        if (state is not DeviceWebSocket client) return;

        // Disconnect if the client hasn't sent a heartbeat in a while
        if (client.MsUntilTimeout <= 0)
        {
            Task.Run(() => client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Heartbeat timeout", CancellationToken.None));
        }
    }

    public void Dispose()
    {
        // Dispose the timer
        try { _heartbeatTimer.Dispose(); } catch { }
    }
}
