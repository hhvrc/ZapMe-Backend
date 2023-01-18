using System.Diagnostics;
using ZapMe.Helpers;

namespace ZapMe.Tests.Helpers.ScopedDelayLockTests;

public sealed class DelayTests
{
    [Fact]
    public async void DelayHundredMilliseconds()
    {
        Stopwatch sw = new Stopwatch();

        sw.Start();
        {
            await using ScopedDelayLock sdl = new ScopedDelayLock(TimeSpan.FromMilliseconds(100));
        }
        sw.Stop();

        Assert.InRange(sw.ElapsedMilliseconds, 80, 120);
    }
}
