using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class StringUtils
{
    private static ReadOnlySpan<char> UrlSafeChars => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

    private static int GetElementsCount(int length)
    {
        int fullCycles = length / 16;
        length -= fullCycles * 16;

        int wasteCycles = length / 5;
        length -= wasteCycles * 5;

        if (length > 0)
        {
            wasteCycles++;
        }

        return (fullCycles * 3) + wasteCycles;
    }

    private static void FillUrlSafeRandomString(Span<char> buffer, int length)
    {
        int elements = GetElementsCount(length);

        int[] rented = ArrayPool<int>.Shared.Rent(elements);
        Span<int> data = new Span<int>(rented);

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(MemoryMarshal.AsBytes(data));
        }

        int bits, i = 0, offset = 0;
        for (; (offset + 16) <= length; i += 3, offset += 16)
        {
            int spareBits;

            bits = data[i + 0];
            buffer[offset + 00] = UrlSafeChars[(bits >> 00) & 0x3F];
            buffer[offset + 01] = UrlSafeChars[(bits >> 06) & 0x3F];
            buffer[offset + 02] = UrlSafeChars[(bits >> 12) & 0x3F];
            buffer[offset + 03] = UrlSafeChars[(bits >> 18) & 0x3F];
            buffer[offset + 04] = UrlSafeChars[(bits >> 24) & 0x3F];
            spareBits = (bits >> 30) & 0x03;

            bits = data[i + 1];
            buffer[offset + 05] = UrlSafeChars[(bits >> 00) & 0x3F];
            buffer[offset + 06] = UrlSafeChars[(bits >> 06) & 0x3F];
            buffer[offset + 07] = UrlSafeChars[(bits >> 12) & 0x3F];
            buffer[offset + 08] = UrlSafeChars[(bits >> 18) & 0x3F];
            buffer[offset + 09] = UrlSafeChars[(bits >> 24) & 0x3F];
            spareBits |= (bits >> 28) & 0x0C;

            bits = data[i + 2];
            buffer[offset + 10] = UrlSafeChars[(bits >> 00) & 0x3F];
            buffer[offset + 11] = UrlSafeChars[(bits >> 06) & 0x3F];
            buffer[offset + 12] = UrlSafeChars[(bits >> 12) & 0x3F];
            buffer[offset + 13] = UrlSafeChars[(bits >> 18) & 0x3F];
            buffer[offset + 14] = UrlSafeChars[(bits >> 24) & 0x3F];
            spareBits |= (bits >> 26) & 0x30;

            buffer[offset + 15] = UrlSafeChars[spareBits];
        }

        for (; (offset + 5) <= length; offset += 5)
        {
            bits = data[i++];
            buffer[offset + 00] = UrlSafeChars[(bits >> 00) & 0x3F];
            buffer[offset + 01] = UrlSafeChars[(bits >> 06) & 0x3F];
            buffer[offset + 02] = UrlSafeChars[(bits >> 12) & 0x3F];
            buffer[offset + 03] = UrlSafeChars[(bits >> 18) & 0x3F];
            buffer[offset + 04] = UrlSafeChars[(bits >> 24) & 0x3F];
        }

        int remainder = length - offset;
        if (remainder > 0)
        {
            bits = data[i++];
            do
            {
                buffer[offset++] = UrlSafeChars[(bits >> (--remainder * 6)) & 0x3F];
            }
            while (remainder > 0);
        }

        ArrayPool<int>.Shared.Return(rented);
    }

    /// <summary>
    /// Generates a cryptographically secure random string of <paramref name="length"/> length, consisting of these characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_
    /// </summary>
    /// <param name="length">Length of the generated string</param>
    /// <returns></returns>
    public static string GenerateUrlSafeRandomString(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than or equal to 0");
        }

        if (length == 0)
        {
            return String.Empty;
        }

        return String.Create(length, length, FillUrlSafeRandomString);
    }
}
