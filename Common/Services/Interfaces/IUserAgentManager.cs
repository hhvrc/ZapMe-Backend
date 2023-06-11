using ZapMe.Database.Models;

namespace ZapMe.Services.Interfaces;

public interface IUserAgentManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userAgent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserAgentEntity> EnsureCreatedAsync(string userAgent, CancellationToken cancellationToken = default);
}
