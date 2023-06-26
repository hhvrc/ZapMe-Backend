using ZapMe.Database.Models;
using ZapMe.Enums;

namespace ZapMe.Services.Interfaces;

public interface IUserManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserEntity?> GetByIdAsync(Guid requestingUserId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserEntity?> GetByUserNameAsync(Guid requestingUserId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CreateFriendRequestAsync(Guid requestingUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AcceptFriendRequestAsync(Guid requestingUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteFriendRequestAsync(Guid requestingUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the relation type from one user to another, and sets the opposite relation if needed.
    /// <para>If A befriends B, then B becomes friend with A, this overrides any previous relation.</para>
    /// <para>If A blocks B, then B becomes nothing to A (except if A has B blocked, then the block remains).</para>
    /// <para>If A becomes nothing to B, B becomes nothing to A (except if B has A blocked, then the block remains).</para>
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="relationType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetUserRelationTypeAsync(Guid requestingUserId, Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken = default);
}