using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IEmailVerificationRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newEmail"></param>
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<EmailVerificationRequestEntity> CreateAsync(Guid userId, string newEmail, string tokenHash, CancellationToken cancellationToken = default);
}