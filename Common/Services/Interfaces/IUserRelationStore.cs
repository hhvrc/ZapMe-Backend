using ZapMe.Database.Models;

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
    Task<UserRelationEntity> CreateAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default);
}
