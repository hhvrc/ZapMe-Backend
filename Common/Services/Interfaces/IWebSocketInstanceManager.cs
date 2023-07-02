using ZapMe.Websocket;

namespace ZapMe.Services.Interfaces;

public interface IWebSocketInstanceManager
{
    /// <summary>
    /// 
    /// </summary>
    const string DefaultRemovalReason = "Forcefully removed";

    ulong OnlineCount { get; }

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
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveClientAsync(Guid clientSessionId, string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DisconnectAllClientsAsync(Guid userId, string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DisconnectEveryoneAsync(string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);
}
