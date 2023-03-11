using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class Utils
{
    public static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

        Span<byte> data = stackalloc byte[(length * 3 / 4) + 4];

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }

        Span<char> buffer = stackalloc char[length + 3];

        for (int i = 0, j = 0; j < length; i += 3, j += 4)
        {
            int bitBuffer = (data[i + 2] << 16) | (data[i + 1] << 8) | data[i];

            buffer[j + 0] = chars[bitBuffer & 0x3F];
            buffer[j + 1] = chars[(bitBuffer >> 6) & 0x3F];
            buffer[j + 2] = chars[(bitBuffer >> 12) & 0x3F];
            buffer[j + 3] = chars[bitBuffer >> 18];
        }

        return new string(buffer[..length]);
    }
}
