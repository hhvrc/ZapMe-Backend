namespace Microsoft.AspNetCore.Http;

public static class PathStringExtensions
{
    public static bool StartsWithSegments(this PathString pathString, params string[] segments)
    {
        if (!pathString.HasValue)
        {
            return false;
        }

        string path = pathString.Value!;

        int startPos = 0;
        int prefixOffset = path.AsSpan().IndexOf(stackalloc char[] { ':', '/', '/' }, StringComparison.Ordinal);
        if (prefixOffset > 0)
        {
            // Get the next slash after the prefix
            startPos = path.AsSpan(prefixOffset + 3).IndexOf('/');
        }

        foreach (string p in segments)
        {
            if (path.AsSpan(startPos).StartsWith(p.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

