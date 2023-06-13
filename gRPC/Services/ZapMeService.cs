using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ZapMe.Database;

namespace ZapMe.gRPC.Services;

public class ZapMeGrpcServiceImpl : ZapMeGrpcService.ZapMeGrpcServiceBase
{
    private uint _clientRtt;
    private uint _serverRtt;
    private bool _isAuthorized;
    private readonly System.Threading.Channels.Channel<ServerEvent> _serverEventChannel;
    private readonly System.Threading.Channels.Channel<SessionServerMessage> _serverSessionMessageChannel;
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<ZapMeGrpcServiceImpl> _logger;

    public ZapMeGrpcServiceImpl(DatabaseContext dbContext, ILogger<ZapMeGrpcServiceImpl> logger)
    {
        _serverEventChannel = System.Threading.Channels.Channel.CreateBounded<ServerEvent>(new System.Threading.Channels.BoundedChannelOptions(1000)
        {
            FullMode = System.Threading.Channels.BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task CloseConnectionAsync()
    {
        // Closing the channel will cause the Authorize method to exit, we can then close the connection.
        _serverEventChannel.Writer.Complete();
        return Task.CompletedTask;
    }

    public override async Task PulseCheck(IAsyncStreamReader<ClientPulse> requestStream, IServerStreamWriter<ServerPulse> responseStream, ServerCallContext context)
    {
        Stopwatch stopwatch = new Stopwatch();

        // Pulse
        if (!await requestStream.MoveNext(context.CancellationToken))
        {
            await CloseConnectionAsync();
            return;
        }

        stopwatch.Start();

        // Pulse ACK
        await responseStream.WriteAsync(new ServerPulse { Interval = 30 }, context.CancellationToken);

        // Pulse FIN
        if (!await requestStream.MoveNext(context.CancellationToken))
        {
            await CloseConnectionAsync();
            return;
        }
        stopwatch.Stop();

        _clientRtt = requestStream.Current!.MeasuredRtt;
        _serverRtt = (uint)stopwatch.ElapsedMilliseconds;

        // Pulse FIN ACK
        await responseStream.WriteAsync(new ServerPulse { MeasuredRtt = _serverRtt, Interval = _clientRtt }, context.CancellationToken);
    }

    public override async Task Authorize(AuthorizeRequest request, IServerStreamWriter<ServerEvent> responseStream, ServerCallContext context)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == Guid.Parse(request.SessionToken), context.CancellationToken);
        if (session == null)
        {
            await CloseConnectionAsync();
            return;
        }

        _isAuthorized = true;
        try
        {
            while (await _serverEventChannel.Reader.WaitToReadAsync(context.CancellationToken))
            {
                while (_serverEventChannel.Reader.TryRead(out ServerEvent? serverEvent))
                {
                    await responseStream.WriteAsync(serverEvent, context.CancellationToken);
                }

                await Task.Delay(100);
            }
        }
        finally
        {
            _isAuthorized = false;
        }
    }

    public override async Task Session(IAsyncStreamReader<SessionMessage> requestStream, IServerStreamWriter<SessionServerMessage> responseStream, ServerCallContext context)
    {
        if (!_isAuthorized) return;

        CancellationTokenSource combinedCancellation = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        CancellationToken combinedCancellationToken = combinedCancellation.Token;
        async Task readFunc()
        {
            while (await requestStream.MoveNext(combinedCancellationToken))
            {
                var message = requestStream.Current!;

                switch (message.PayloadCase)
                {
                    case SessionMessage.PayloadOneofCase.InitSession:
                        break;
                    case SessionMessage.PayloadOneofCase.Webrtc:
                        break;
                    case SessionMessage.PayloadOneofCase.Device:
                        break;
                    case SessionMessage.PayloadOneofCase.Voice:
                        break;
                    case SessionMessage.PayloadOneofCase.None:
                    default:
                        break;
                }
            }
            lock (combinedCancellation) combinedCancellation.Cancel();
        }
        async Task writeFunc()
        {
            while (await _serverSessionMessageChannel.Reader.WaitToReadAsync(combinedCancellationToken))
            {
                while (_serverSessionMessageChannel.Reader.TryRead(out SessionServerMessage? serverMessage))
                {
                    await responseStream.WriteAsync(serverMessage, combinedCancellationToken);
                }
            }
            lock (combinedCancellation) combinedCancellation.Cancel();
        }

        await Task.WhenAll(readFunc(), writeFunc());
    }
}
