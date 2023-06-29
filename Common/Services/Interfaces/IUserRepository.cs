using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// Repository to perform CRUD operations on users
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Creates a ordinary user account that will need to accept the privacy policy and terms of service.
    /// </summary>
    Task<UserEntity> CreateUserAsync(UserCreationDto newUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user account that is verified and will never need to accept the privacy policy or terms of service.
    /// </summary>
    Task<UserEntity> CreateSystemUserAsync(SystemUserCreationDto newUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their id, returns a <see cref="UserEntity"/> with all navigation properties populated, or null if the user does not exist.
    /// </summary>
    Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their name, returns a <see cref="UserEntity"/> with all navigation properties populated, or null if the user does not exist.
    /// </summary>
    Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email, returns a <see cref="UserEntity"/> with all navigation properties populated, or null if the user does not exist.
    /// </summary>
    Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);


}