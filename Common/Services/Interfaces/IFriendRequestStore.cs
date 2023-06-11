using ZapMe.Database.Models;

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
}
