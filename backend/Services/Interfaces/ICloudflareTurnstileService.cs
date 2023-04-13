using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface ICloudFlareTurnstileService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="remoteIpAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CloudflareTurnstileVerifyResponse> VerifyUserResponseTokenAsync(string token, string remoteIpAddress, CancellationToken cancellationToken = default);
}
