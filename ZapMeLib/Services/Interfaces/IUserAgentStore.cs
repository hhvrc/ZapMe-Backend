using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IUserAgentStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="length"></param>
    /// <param name="value">Trunkated value</param>
    /// <param name="operatingSystem"></param>
    /// <param name="device"></param>
    /// <param name="browser"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<UserAgentEntity> CreateAsync(string hash, uint length, string value, string operatingSystem, string device, string browser, CancellationToken cancellationToken = default);
}
