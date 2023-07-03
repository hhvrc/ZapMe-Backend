using System.Net.WebSockets;
using ZapMe.Websocket;

namespace ZapMe.Services.Interfaces;

public interface IWebSocketInstanceManager
{
    /// <summary>
    /// 
    /// </summary>
    const string DefaultRemovalReason = "Forcefully removed";

    uint OnlineCount { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RegisterClientAsync(WebSocketClient client, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientSessionId"></param>
    /// <param name="closeStatus"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveClientAsync(Guid clientSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="closeStatus"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DisconnectUserClientsAsync(Guid userId, WebSocketCloseStatus closeStatus, string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="closeStatus"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DisconnectAllClientsAsync(WebSocketCloseStatus closeStatus, string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientSessionId"></param>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RunActionOnClientAsync(Guid clientSessionId, Func<WebSocketClient, Task> action, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RunActionOnUserClientsAsync(Guid userId, Func<WebSocketClient, Task> action, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RunActionOnAllClientsAsync(Func<WebSocketClient, Task> action, CancellationToken cancellationToken);
}
