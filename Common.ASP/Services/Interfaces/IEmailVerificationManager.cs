using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface IEmailVerificationManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ErrorDetails?> InitiateEmailVerificationAsync(UserEntity user, string newEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ErrorDetails?> CompleteEmailVerificationAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<int> RemoveExpiredRequestsAsync(CancellationToken cancellationToken = default);
}
