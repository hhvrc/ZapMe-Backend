using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _dbContext;

    public UserRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserEntity> CreateUserAsync(UserCreationDto newUser, CancellationToken cancellationToken)
    {
        string passwordHash = PasswordUtils.HashPassword(newUser.Password);

        UserEntity user = new UserEntity
        {
            Name = newUser.Username,
            Email = newUser.Email,
            EmailVerified = newUser.EmailVerified,
            PasswordHash = passwordHash,
            AcceptedPrivacyPolicyVersion = newUser.AcceptedPrivacyPolicyVersion,
            AcceptedTermsOfServiceVersion = newUser.AcceptedTermsOfServiceVersion,
            ProfileAvatarId = newUser.ProfileAvatarImageId,
            ProfileBannerId = newUser.ProfileBannerImageId,
            Status = UserStatus.Online,
            StatusText = String.Empty
        };

        // Create account
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Read account back from database to get all navigation properties, this should not be null
        user = (await GetUserByIdAsync(user.Id, cancellationToken))!;

        return user;
    }

    public Task<UserEntity> CreateSystemUserAsync(SystemUserCreationDto newUser, CancellationToken cancellationToken)
    {
        return CreateUserAsync(
            new(
                newUser.Username,
                newUser.Email,
                EmailVerified: true,
                newUser.Password,
                AcceptedPrivacyPolicyVersion: UInt32.MaxValue,
                AcceptedTermsOfServiceVersion: UInt32.MaxValue,
                ProfileAvatarImageId: null,
                ProfileBannerImageId: null
            ),
            cancellationToken
            );
    }

    private IQueryable<UserEntity> QueryBase =>
        _dbContext
            .Users
            .AsNoTracking()
            .Include(u => u.ProfileAvatar)
            .Include(u => u.ProfileBanner)
            .Include(u => u.PasswordResetRequest)
            .Include(u => u.EmailVerificationRequest)
            .Include(u => u.Sessions)
            .Include(u => u.LockOuts)
            .Include(u => u.UserRoles)
            .Include(u => u.RelationsOutgoing)
            .Include(u => u.RelationsIncoming)
            .Include(u => u.SSOConnections);

    public Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return QueryBase.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return QueryBase.FirstOrDefaultAsync(u => u.Name == userName, cancellationToken);
    }

    public Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return QueryBase.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
