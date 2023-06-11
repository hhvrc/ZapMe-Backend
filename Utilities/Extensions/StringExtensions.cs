﻿using System.Runtime.CompilerServices;

namespace System;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Obscure(ReadOnlySpan<char> str, Span<char> buffer)
    {
        if (str.Length > 4)
        {
            buffer[0] = str[0];
            buffer[1..^1].Fill('*');
            buffer[^1] = str[^1];
        }
        else
        {
            buffer.Fill('*');
        }
    }

    public static string Obscure(this string str)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(str));
        if (str.Length == 0) return String.Empty;

        Span<char> buffer = stackalloc char[str.Length];

        Obscure(str, buffer);

        return new string(buffer);
    }

    public static string ObscureExceptLast(this string str, char seperator)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(str));
        if (str.Length == 0) return String.Empty;

        ReadOnlySpan<char> span = str.AsSpan();
        Span<char> buffer = stackalloc char[str.Length];

        int lastSep = span.LastIndexOf(seperator);
        if (lastSep < 0)
        {
            Obscure(span, buffer);
        }
        else
        {
            Obscure(span[..lastSep], buffer[..lastSep]);
            span[lastSep..].CopyTo(buffer[lastSep..]);
        }

        return new string(buffer);
    }
}
