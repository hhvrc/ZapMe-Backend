using BCrypt.Net;

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
}
