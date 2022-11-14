using BCrypt.Net;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (password == null) throw new NullReferenceException(nameof(password));

        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA512, 15);
    }

    public bool CheckPassword(string submittedPassword, string hashedPassword)
    {
        if (submittedPassword == null) throw new NullReferenceException(nameof(submittedPassword));
        if (hashedPassword == null) throw new NullReferenceException(nameof(hashedPassword));

        return BCrypt.Net.BCrypt.EnhancedVerify(submittedPassword, hashedPassword, HashType.SHA512);
    }
}