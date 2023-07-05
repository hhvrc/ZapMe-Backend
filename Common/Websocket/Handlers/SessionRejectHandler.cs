using fbs.session;

namespace ZapMe.Websocket;

partial class WebSocketClient
{
    private Task<bool> HandleSessionRejectAsync(SessionReject msg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}