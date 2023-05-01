using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IGoogleReCaptchaService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="responseToken"></param>
    /// <param name="remoteIpAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GoogleReCaptchaVerifyResponse> VerifyUserResponseTokenAsync(string responseToken, string? remoteIpAddress, CancellationToken cancellationToken = default);
}
