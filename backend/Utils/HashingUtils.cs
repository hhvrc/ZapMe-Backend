using System.Security.Cryptography;
using System.Text;

namespace ZapMe.Utils;

public static class HashingUtils
{
    public static string Sha256_String(string str)
    {
        return EncodingUtils.ToHex(SHA256.HashData(Encoding.UTF8.GetBytes(str)));
    }

    public static byte[] Sha256_Bytes(byte[] bytes)
    {
        return SHA256.HashData(bytes);
    }
}
