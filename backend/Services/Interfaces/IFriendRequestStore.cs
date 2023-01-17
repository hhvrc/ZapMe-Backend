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
    Task<FriendRequestEntity> CreateAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    IAsyncEnumerable<FriendRequestEntity> ListByUserAsync(Guid userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="senderId"></param>
    /// <returns></returns>
    IAsyncEnumerable<FriendRequestEntity> ListBySenderAsync(Guid senderId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiverId"></param>
    /// <returns></returns>
    IAsyncEnumerable<FriendRequestEntity> ListByReceiverAsync(Guid receiverId);
}
