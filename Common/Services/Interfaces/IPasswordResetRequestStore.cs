using ZapMe.Database.Models;

namespace ZapMe.Services.Interfaces;

public interface IPasswordResetRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<UserPasswordResetRequestEntity> UpsertAsync(Guid accountId, string tokenHash, CancellationToken cancellationToken = default);
}
