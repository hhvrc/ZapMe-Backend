using NetMQ;
using NetMQ.Sockets;
using System.Net.WebSockets;
using System.Security.Claims;
using ZapMe.FlatMsgs;

namespace ZapMe.Websocket;

public sealed class WebSocketInstance : IDisposable
{
    public static async Task<WebSocketInstance?> CreateAsync(WebSocketManager wsManager, ClaimsPrincipal user, ILogger<WebSocketInstance> logger)
    {
        var ws = await wsManager.AcceptWebSocketAsync();
        if (ws == null) return null;

        return new WebSocketInstance(ws, user, logger);
    }

    private const int _WebSocketBufferSize = 4096;
    private const int _ZeroMqBufferSize = 4096;
    private readonly ILogger<WebSocketInstance> _logger;
    private readonly WebSocket _webSocket;
    private readonly byte[] _webSocketBuffer;
    private ArraySegment<byte> _webSocketBufferData;
    private WebSocketMessageType _webSocketMessageType;
    private readonly SubscriberSocket _zmqSub;
    private readonly byte[] _zmqBuffer;
    private ArraySegment<byte> _zmqBufferData;
    private readonly ClaimsPrincipal _user;

    private WebSocketInstance(WebSocket webSocket, ClaimsPrincipal user, ILogger<WebSocketInstance> logger)
    {
        _webSocket = webSocket;
        _webSocketBuffer = new byte[_WebSocketBufferSize];
        _zmqSub = new SubscriberSocket();
        _zmqBuffer = new byte[_ZeroMqBufferSize];
        _user = user;
        _logger = logger;

        // ZeroMQ Configuration
        _zmqSub.Connect("someaddr");
        _zmqSub.Subscribe("A"); // User specific
        _zmqSub.Subscribe("B"); // Group specific
        _zmqSub.Subscribe("C"); // Global

        // WebSocket Configuration
    }

    public async Task RunAsync(CancellationToken cs)
    {
        // TODO: mark person as online

        try
        {
            await Task.WhenAll(RunZeroMqAsync(cs), RunWebSocketAsync(cs));
        }
        finally
        {
            // TODO: mark person as offline
        }
    }

    private async Task RunZeroMqAsync(CancellationToken cs)
    {
        while (_webSocket.State == WebSocketState.Open)
        {
            if (await ReadZeroMqBytesAsync(cs))
            {
                await ProcessZeroMqBytesAsync(cs);
            }
        }
    }

    private async Task<bool> ReadZeroMqBytesAsync(CancellationToken cs)
    {
        _zmqBufferData = new ArraySegment<byte>(_zmqBuffer);

        byte[] data;
        bool endOfMsg;
        int bufferOffset = 0;

        do
        {
            (data, endOfMsg) = await _zmqSub.ReceiveFrameBytesAsync(cs);
            if (bufferOffset + data.Length > _ZeroMqBufferSize)
            {
                // TODO: possibly log message too big

                return false;
            }

            Buffer.BlockCopy(data, 0, _zmqBuffer, bufferOffset, data.Length);
            bufferOffset += data.Length;
        }
        while (!endOfMsg);

        _zmqBufferData = _zmqBufferData[..bufferOffset];

        return bufferOffset > 0;
    }

    private async Task ProcessZeroMqBytesAsync(CancellationToken cs)
    {
        await Task.CompletedTask; // TODO: implement me
    }

    private async Task RunWebSocketAsync(CancellationToken cs)
    {
        while (_webSocket.State == WebSocketState.Open)
        {
            if (!await ReadWebSocketBytesAsync(cs)) break;

            if (_webSocketMessageType switch
            {
                WebSocketMessageType.Text => await ProcessWebSocketTextAsync(cs),
                WebSocketMessageType.Binary => await ProcessWebSocketBinaryAsync(cs),
                _ => false
            })
            {
                break;
            }
        }
    }

    private async Task<bool> ReadWebSocketBytesAsync(CancellationToken cs)
    {
        _webSocketBufferData = new ArraySegment<byte>(_webSocketBuffer);

        WebSocketReceiveResult msg = await _webSocket.ReceiveAsync(_webSocketBufferData, cs);

        if (msg.Count <= 0)
        {
            await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload size invalid!", cs);
            return false;
        }

        _webSocketMessageType = msg.MessageType;
        _webSocketBufferData = _webSocketBufferData[..msg.Count];

        if (_webSocketMessageType == WebSocketMessageType.Close)
        {
            await CloseAsync(cs: cs);
            return false;
        }

        if (!msg.EndOfMessage)
        {
            await CloseAsync(WebSocketCloseStatus.MessageTooBig, "Payload size too big!", cs);
            return false;
        }

        return true;
    }

    private Task<bool> ProcessWebSocketTextAsync(CancellationToken cs)
    {
        return Task.FromResult(false); // TODO: implement me
    }
    private async Task<bool> ProcessWebSocketBinaryAsync(CancellationToken cs)
    {
        ClientMsg msg;

        try
        {
            msg = ClientMsg.Serializer.Parse(_webSocketBufferData.ToFlatSharpMemory());
        }
        catch (Exception)
        {
            await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!", cs);
            return false;
        }

        if (!msg.Payload.HasValue)
        {
            await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload invalid!", cs);
            return false;
        }

        switch (msg.Payload.Value.Kind)
        {
            case ClientPayload.ItemKind.SendShock:
                _logger.LogDebug("Shock!");
                break;
            case ClientPayload.ItemKind.SendVibrate:
                _logger.LogDebug("Vibrate!");
                break;
            case ClientPayload.ItemKind.NONE:
            default:
                return false;
        }

        return true;
    }

    public async Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string reason = "ByeBye ❤️", CancellationToken cs = default)
    {
        try
        {
            await _webSocket.CloseAsync(closeStatus, reason, cs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while closing websocket: {0}", ex.Message);
        }
    }

    public void Dispose()
    {
        _webSocket.Dispose();
        _zmqSub.Dispose();
    }
}
