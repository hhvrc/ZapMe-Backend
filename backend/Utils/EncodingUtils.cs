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
}
