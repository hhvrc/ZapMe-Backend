using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class StringUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char GetUrlSafeChar(int bits) => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_"[bits & 0x3F];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AssignUrlSafeCharChunk(Span<char> buffer, int bits)
    {
        buffer[0] = GetUrlSafeChar(bits >> 00);
        buffer[1] = GetUrlSafeChar(bits >> 06);
        buffer[2] = GetUrlSafeChar(bits >> 12);
        buffer[3] = GetUrlSafeChar(bits >> 18);
        buffer[4] = GetUrlSafeChar(bits >> 24);
        return bits >> 30;
    }

    private static int GetElementsCount(int length)
    {
        int fullCycles = (length >> 4) * 3; // (length / 16) * 3
        length &= 0xF; // length % 16

        int wasteCycles = length / 5;
        length -= wasteCycles * 5;

        if (length > 0)
        {
            wasteCycles++;
        }

        return fullCycles + wasteCycles;
    }


    private static void FillUrlSafeRandomString(Span<char> buffer, int length)
    {
        int elements = GetElementsCount(length);

        int[] rented = ArrayPool<int>.Shared.Rent(elements);
        Span<int> data = new Span<int>(rented);

        RandomNumberGenerator.Fill(MemoryMarshal.AsBytes(data));

        int i = 0, offset = 0;
        for (; (offset + 16) <= length; i += 3, offset += 16)
        {
            Span<char> chunk = buffer.Slice(offset, 16);

            int spareBits
                = (AssignUrlSafeCharChunk(chunk[..5], data[i + 0]) << 0)
                | (AssignUrlSafeCharChunk(chunk[5..10], data[i + 1]) << 2)
                | (AssignUrlSafeCharChunk(chunk[10..15], data[i + 2]) << 4);

            chunk[15] = GetUrlSafeChar(spareBits);
        }

        for (; (offset + 5) <= length; i++, offset += 5)
        {
            _ = AssignUrlSafeCharChunk(buffer.Slice(offset, 5), data[i]);
        }

        int remainder = length - offset;
        if (remainder > 0)
        {
            int bits = data[i++];
            do
            {
                buffer[offset++] = GetUrlSafeChar(bits >> (--remainder * 6));
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
