using NetMQ;
using NetMQ.Sockets;
using System.Net.WebSockets;
using System.Security.Claims;

namespace ZapMe.Controllers;

public sealed class WebSocketInstance : IDisposable
{
    public static async Task<WebSocketInstance?> CreateAsync(WebSocketManager wsManager, ClaimsPrincipal user, ILogger<WebSocketInstance> logger)
    {
        var ws = await wsManager.AcceptWebSocketAsync();
        if (ws == null) return null;

        return new WebSocketInstance(ws, user, logger);
    }

    private const int WebSocketBufferSize = 4096;
    private const int ZeroMqBufferSize = 4096;
    private readonly ILogger<WebSocketInstance> _logger;
    private readonly WebSocket _webSocket;
    private readonly byte[] _webSocketBuffer;
    private WebSocketMessageType _webSocketMessageType;
    private readonly SubscriberSocket _zmqSub;
    private readonly byte[] _zmqSubBuffer;
    private readonly ClaimsPrincipal _user;

    private WebSocketInstance(WebSocket webSocket, ClaimsPrincipal user, ILogger<WebSocketInstance> logger)
    {
        _webSocket = webSocket;
        _webSocketBuffer = new byte[WebSocketBufferSize];
        _zmqSub = new SubscriberSocket();
        _zmqSubBuffer = new byte[ZeroMqBufferSize];
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
        byte[] data;
        bool endOfMsg;
        int bufferOffset = 0;

        do
        {
            (data, endOfMsg) = await _zmqSub.ReceiveFrameBytesAsync(cs);
            if (bufferOffset + data.Length > ZeroMqBufferSize)
            {
                // TODO: possibly log message too big

                return false;
            }

            Buffer.BlockCopy(data, 0, _zmqSubBuffer, bufferOffset, data.Length);
            bufferOffset += data.Length;
        }
        while (!endOfMsg);

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
            if (await ReadWebSocketBytesAsync(cs))
            {
                if (_webSocketMessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Byebye :)", cs);
                    return;
                }

                await ProcessWebSocketBytesAsync(cs);
            }
        }
    }

    private async Task<bool> ReadWebSocketBytesAsync(CancellationToken cs)
    {
        int bufferOffset = 0;
        WebSocketReceiveResult msg;

        do
        {
            // Buffer overflow
            if (bufferOffset >= WebSocketBufferSize)
            {
                await CloseAsync(WebSocketCloseStatus.MessageTooBig, $"Payload exceeded max size of {WebSocketBufferSize}", cs);
                return false;
            }

            msg = await _webSocket.ReceiveAsync(new ArraySegment<byte>(_webSocketBuffer, 0, bufferOffset), cs);

            // Null
            if (msg == null)
            {
                // TODO: figure this out
                await CloseAsync(WebSocketCloseStatus.InternalServerError, "Unknown error on receive", cs);
                return false;
            }

            // Set messageType if beginning of message, else check if it has changed mid-message
            if (bufferOffset == 0)
            {
                _webSocketMessageType = msg.MessageType;
            }
            else if (_webSocketMessageType != msg.MessageType)
            {
                // TODO: log this
                await CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Payload type malformed!", cs);
                return false;
            }

            // Increment bytes received
            bufferOffset += msg.Count;
        }
        while (!msg.EndOfMessage);

        return bufferOffset > 0;
    }

    private async Task ProcessWebSocketBytesAsync(CancellationToken cs)
    {
        await Task.CompletedTask; // TODO: implement me
    }

    public async Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string reason = "ByeBye ❤️", CancellationToken cs = default)
    {
        try
        {
            await _webSocket.CloseAsync(closeStatus, reason, cs);
        }
        catch (Exception)
        {
        }
    }

    public void Dispose()
    {
        _webSocket.Dispose();
        _zmqSub.Dispose();
    }
}
