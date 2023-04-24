using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IUserManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="password">User's password, will be hashed with <see cref="Utils.PasswordUtils"/>.</param>
    /// <param name="emailVerified">If true, the user will not be able to login until their email address is verified.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<UserEntity, ErrorDetails>> TryCreateAsync(string name, string email, string password, bool emailVerified, CancellationToken cancellationToken = default);
}