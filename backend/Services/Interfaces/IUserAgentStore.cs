using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IUserAgentStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userAgent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserAgentEntity> EnsureCreatedAsync(string userAgent, CancellationToken cancellationToken = default);
}
