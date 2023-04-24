using ZapMe.Controllers.Api.V1.Models;

namespace ZapMe.Services.Interfaces;

public interface IEmailVerificationManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="newEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ErrorDetails?> InitiateEmailVerificationAsync(string userName, string newEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ErrorDetails?> CompleteEmailVerificationAsync(string token, CancellationToken cancellationToken = default);
}
