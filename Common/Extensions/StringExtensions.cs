namespace System;

public static class StringExtensions
{
    public static string TrimAndMinifyWhiteSpaces(this string str)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(str));
        if (str.Length == 0) return String.Empty;

        ReadOnlySpan<char> span = str.AsSpan().Trim();

        int i = 0, j = 0, len = span.Length;

        Span<char> buffer = new char[len];

        while (i < len)
        {
            char c = span[i++];

            if (Char.IsWhiteSpace(c))
            {
                buffer[j++] = ' ';

                do
                {
                    if (i >= len) goto exit; // Break twice
                }
                while (Char.IsWhiteSpace(c = span[i++]));
            }

            buffer[j++] = c;
        }
    exit:

        return new string(buffer[..j]);
    }
}
