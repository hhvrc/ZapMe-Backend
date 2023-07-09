using System.Net.WebSockets;

namespace ZapMe.Websocket;

public interface IWebSocketHandler
{
    Task RunAsync(Func<string?, Task<WebSocket>> webSocketAcceptFunc, IList<string> requestedSubProtocols, CancellationToken cancellationToken = default);
}