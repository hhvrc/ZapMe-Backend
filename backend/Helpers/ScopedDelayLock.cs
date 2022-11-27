namespace ZapMe.Helpers;

/// <summary>
/// Makes sure that the scope that uses this disposable object will wait a certain amount of time before exiting
/// </summary>
public sealed class ScopedDelayLock : IAsyncDisposable
{
    public static ScopedDelayLock FromSeconds(double seconds, CancellationToken cancellationToken = default) => new ScopedDelayLock(TimeSpan.FromSeconds(seconds), cancellationToken);
    public static ScopedDelayLock FromMinutes(double minutes, CancellationToken cancellationToken = default) => new ScopedDelayLock(TimeSpan.FromMinutes(minutes), cancellationToken);

    /// <summary>
    /// Create a new lock
    /// 
    /// Usage:
    /// await using new ScopedDelayLock(TimeSpan.FromSeconds(5), myCancellationToken);
    /// </summary>
    /// <param name="lockTime">Amount of time to lock the current scope for</param>
    /// <param name="cancellationToken"></param>
    public ScopedDelayLock(TimeSpan lockTime, CancellationToken cancellationToken = default)
    {
        _unlockTime = DateTime.Now.Ticks + lockTime.Ticks;
        _cancellationToken = cancellationToken;
    }

    private readonly long _unlockTime;
    private readonly CancellationToken _cancellationToken;
    
    public async ValueTask DisposeAsync()
    {
        long ticksLeft = _unlockTime - DateTime.Now.Ticks;
        
        if (ticksLeft > 0)
        {
            await Task.Delay(TimeSpan.FromTicks(ticksLeft), _cancellationToken);
        }
    }
}
