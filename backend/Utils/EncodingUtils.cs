using System.Globalization;
using System.Text;

namespace ZapMe.Utils;

public static class EncodingUtils
{
    public static string TransCode(ReadOnlySpan<char> str, Encoding from, Encoding to)
    {
        int bufferLen = from.GetByteCount(str);
        byte[] buffer = new byte[bufferLen];

        int encodedLen = from.GetBytes(str, buffer);

        Encoding.Convert(from, to, buffer);

        return to.GetString(buffer);
    }

    public static string ToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
    }

    public static byte[] FromHex(string hex)
    {
        if (hex.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string must have an even number of characters");
        }

        byte[] bytes = new byte[hex.Length / 2];

        ReadOnlySpan<char> hexSpan = hex.AsSpan();

        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Byte.Parse(hexSpan.Slice(i * 2, 2), NumberStyles.HexNumber);
        }

        return bytes;
    }
}
