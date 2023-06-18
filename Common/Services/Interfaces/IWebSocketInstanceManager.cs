using ZapMe.Websocket;

namespace ZapMe.Services.Interfaces;

public interface IWebSocketInstanceManager
{
    /// <summary>
    /// 
    /// </summary>
    const string DefaultRemovalReason = "Forcefully removed";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="instanceId"></param>
    /// <param name="instance"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RegisterInstanceAsync(Guid userId, string instanceId, WebSocketInstance instance, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveInstanceAsync(string instanceId, string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveAllInstancesAsync(Guid userId, string reason = DefaultRemovalReason, CancellationToken cancellationToken = default);
}
