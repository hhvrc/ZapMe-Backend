using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IUserStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> TryCreateAsync(UserEntity user, CancellationToken cancellationToken = default);
}