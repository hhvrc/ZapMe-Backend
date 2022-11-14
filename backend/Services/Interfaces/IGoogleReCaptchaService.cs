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
    /// <param name="reCaptchaToken"></param>
    /// <param name="remoteIpAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GoogleReCaptchaVerifyResponse> VerifyUserResponseTokenAsync(string reCaptchaToken, string remoteIpAddress, CancellationToken cancellationToken = default);
}
