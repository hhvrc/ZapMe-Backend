namespace ZapMe.Services.Interfaces;

public interface IPasswordHasher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    string HashPassword(string password);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="submittedPassword"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    bool CheckPassword(string submittedPassword, string hashedPassword);
}