using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class StringUtils
{
    private static void FillUrlSafeRandomString(Span<char> buffer, int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

        int[] array = ArrayPool<int>.Shared.Rent((length / 5) + 1);
        try
        {
            Span<int> data = new(array);

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(MemoryMarshal.AsBytes(data));
            }

            int i = 0;
            int j = 0;
            do
            {
                int bitBuffer = data[i++];

                buffer[j + 0] = chars[bitBuffer & 0x3F];
                buffer[j + 1] = chars[(bitBuffer >> 6) & 0x3F];
                buffer[j + 2] = chars[(bitBuffer >> 12) & 0x3F];
                buffer[j + 3] = chars[(bitBuffer >> 18) & 0x3F];
                buffer[j + 4] = chars[(bitBuffer >> 24) & 0x3F];

                j += 5;
            }
            while ((j + 5) < length);

            int remainder = length - j;
            if (remainder == 0)
            {
                return;
            }

            int lastBitBuffer = data[i];
            do
            {
                buffer[j++] = chars[(lastBitBuffer >> (--remainder * 6)) & 0x3F];
            }
            while (remainder > 0);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(array);
        }
    }

    /// <summary>
    /// Generates a cryptographically secure random string of <paramref name="length"/> length, consisting of these characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_
    /// </summary>
    /// <param name="length">Length of the generated string</param>
    /// <returns></returns>
    public static string GenerateUrlSafeRandomString(int length)
    {
        return String.Create(length, length, FillUrlSafeRandomString);
    }
}
