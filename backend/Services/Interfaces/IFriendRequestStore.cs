using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IFriendRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="senderId"></param>
    /// <param name="receiverId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FriendRequestEntity?> CreateAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FriendRequestEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="senderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FriendRequestEntity[]> ListBySenderAsync(Guid senderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiverId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FriendRequestEntity[]> ListByReceiverAsync(Guid receiverId, CancellationToken cancellationToken = default);
}
