using BCrypt.Net;
using System.Security.Cryptography;

namespace ZapMe.Utils;

public static class PasswordUtils
{
    public static string HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA512, 15);
    }
    
    public static bool CheckPassword(string submittedPassword, string hashedPassword)
    {
        ArgumentNullException.ThrowIfNull(submittedPassword);
        ArgumentNullException.ThrowIfNull(hashedPassword);
        
        return BCrypt.Net.BCrypt.EnhancedVerify(submittedPassword, hashedPassword, HashType.SHA512);
    }

    private const string _Chars = "abcdefghijklmnopqrstuvwxyz" +
                                  "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                  "01234567890123456789" +
                                  "!#$%()*+,-./:;<=>?@[]^_{}~";
    public static string GeneratePassword(int length = 32)
    {
        return String.Create(length, false, (span, _) =>
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            
            Span<byte> data = new byte[length];
            rng.GetBytes(data);

            for (int i = 0; i < length; i++)
            {
                span[i] = _Chars[data[i] % _Chars.Length];
            }
        });
    }
}
