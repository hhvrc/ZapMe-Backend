using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ZapMe.Helpers;

public sealed class SlidingWindow
{
    private readonly object _lock = new object();
    private readonly long _windowSizeMs;
    private readonly ulong _maxUnitsPerWindow;

    private long _windowStartTicks = Stopwatch.GetTimestamp();
    private ulong _unitsPreviousWindow = 0;
    private ulong _unitsCurrentWindow = 0;

    public SlidingWindow(long windowSizeMs, ulong maxUnitsPerWindow)
    {
        if (windowSizeMs <= 0)
            throw new ArgumentOutOfRangeException(nameof(windowSizeMs), "Window size must be greater than 0");
        if (maxUnitsPerWindow == 0)
            throw new ArgumentOutOfRangeException(nameof(maxUnitsPerWindow), "Max requests per window must be greater than 0");

        _windowSizeMs = windowSizeMs * TimeSpan.TicksPerMillisecond;
        _maxUnitsPerWindow = maxUnitsPerWindow;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool RequestConforms(ulong unitsRequested = 1)
    {
        bool requestConforms = false;

        lock (_lock)
        {
            long nowTicks = Stopwatch.GetTimestamp();
            long elapsedTicks = nowTicks - _windowStartTicks;

            if (elapsedTicks >= _windowSizeMs)
            {
                if (elapsedTicks >= _windowSizeMs * 2)
                {
                    _windowStartTicks = nowTicks;
                    _unitsPreviousWindow = 0;
                    _unitsCurrentWindow = 0;
                    elapsedTicks = 0;
                }
                else
                {
                    _windowStartTicks += _windowSizeMs;
                    _unitsPreviousWindow = _unitsCurrentWindow;
                    _unitsCurrentWindow = 0;

                    elapsedTicks -= _windowSizeMs;
                }

                _windowStartTicks = nowTicks;
                _unitsPreviousWindow = _unitsCurrentWindow;
                _unitsCurrentWindow = 0;
            }

            double weightedRequestCount = (_unitsPreviousWindow * ((double)_windowSizeMs - elapsedTicks) / _windowSizeMs) + _unitsCurrentWindow + unitsRequested;
            if (weightedRequestCount <= _maxUnitsPerWindow)
            {
                _unitsCurrentWindow += unitsRequested;
                requestConforms = true;
            }

            return requestConforms;
        }
    }
}
