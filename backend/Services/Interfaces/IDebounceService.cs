namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IDebounceService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task<bool> IsDisposableEmailAsync(string email, CancellationToken cancellation = default);
}