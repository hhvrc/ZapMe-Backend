using System.Buffers;
using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class StringUtils
{
    private const string _Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
    private static ReadOnlySpan<char> CharsSpan => _Chars.AsSpan();

    private static char GetUrlSafeChar(byte bits) => CharsSpan[bits & 0x3F];

    private static void FillUrlSafeRandomString(Span<char> buffer, int length)
    {
        byte[] rented = ArrayPool<byte>.Shared.Rent((length * 3 / 4) + 3);
        Span<byte> data = new Span<byte>(rented);

        RandomNumberGenerator.Fill(data);

        int strPos = 0;
        int dataPos = 0;
        int batchLength = length & ~3;
        for (; (strPos + 4) <= batchLength; dataPos += 3, strPos += 4)
        {
            buffer[strPos + 0] = GetUrlSafeChar(data[dataPos + 0]);
            buffer[strPos + 1] = GetUrlSafeChar(data[dataPos + 1]);
            buffer[strPos + 2] = GetUrlSafeChar(data[dataPos + 2]);
            buffer[strPos + 3] = GetUrlSafeChar((byte)((data[dataPos + 0] >> 6) | (data[dataPos + 1] >> 4) | (data[dataPos + 2] >> 2)));
        }

        switch (length - batchLength)
        {
            case 1:
                buffer[strPos + 0] = GetUrlSafeChar(data[dataPos + 0]);
                break;
            case 2:
                buffer[strPos + 0] = GetUrlSafeChar(data[dataPos + 0]);
                buffer[strPos + 1] = GetUrlSafeChar(data[dataPos + 1]);
                break;
            case 3:
                buffer[strPos + 0] = GetUrlSafeChar(data[dataPos + 0]);
                buffer[strPos + 1] = GetUrlSafeChar(data[dataPos + 1]);
                buffer[strPos + 2] = GetUrlSafeChar(data[dataPos + 2]);
                break;
            default:
                break;
        }

        ArrayPool<byte>.Shared.Return(rented);
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
