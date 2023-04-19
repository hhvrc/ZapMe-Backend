using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IMailAddressVerificationRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newEmail"></param>
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MailAddressChangeRequestEntity> CreateAsync(Guid userId, string newEmail, string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MailAddressChangeRequestEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MailAddressChangeRequestEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> DeleteByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}