using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IUserRelationStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserRelationEntity?> CreateAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserRelationEntity[]> ListOutgoingAsync(Guid sourceUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetUserId"></param>
    /// <param name="relationType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserRelationEntity[]> ListIncomingByTypeAsync(Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="relationType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetRelationTypeAsync(Guid sourceUserId, Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="nickName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetNickNameAsync(Guid sourceUserId, Guid targetUserId, string nickName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="notes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetNotesAsync(Guid sourceUserId, Guid targetUserId, string notes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="relationType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Amount of relations removed</returns>
    Task<int> PurgeAsync(Guid userId, UserRelationType? relationType, CancellationToken cancellationToken);
}
