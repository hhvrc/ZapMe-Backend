namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IDebounceService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task<bool> IsDisposableEmailAsync(string emailAddress, CancellationToken cancellation = default);
}