using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface ICloudflareTurnstileService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="responseToken"></param>
    /// <param name="remoteIpAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CloudflareTurnstileVerifyResponseDto> VerifyUserResponseTokenAsync(string responseToken, string? remoteIpAddress, CancellationToken cancellationToken = default);
}
