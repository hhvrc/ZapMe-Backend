namespace ZapMe.Helpers;

public sealed class TimeLock : IAsyncDisposable
{
    public static TimeLock FromSeconds(double seconds, CancellationToken cancellationToken = default) => new TimeLock(TimeSpan.FromSeconds(seconds), cancellationToken);
    public static TimeLock FromMinutes(double minutes, CancellationToken cancellationToken = default) => new TimeLock(TimeSpan.FromMinutes(minutes), cancellationToken);
    
    public TimeLock(TimeSpan lockTime, CancellationToken cancellationToken = default)
    {
        _unlockTime = DateTime.Now + lockTime;
        _cancellationToken = cancellationToken;
    }

    private readonly DateTime _unlockTime;
    private readonly CancellationToken _cancellationToken;

    public TimeSpan TimeLeft => _unlockTime - DateTime.Now;

    public async ValueTask DisposeAsync()
    {
        TimeSpan waitTime = TimeLeft;
        if (waitTime > TimeSpan.Zero)
        {
            await Task.Delay(waitTime, _cancellationToken);
        }
    }
}
