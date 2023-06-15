using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using ZapMe.Database.Models;

namespace ZapMe.Database;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public required DbSet<UserEntity> Users { get; set; }
    public required DbSet<ImageEntity> Images { get; set; }
    public required DbSet<SessionEntity> Sessions { get; set; }
    public required DbSet<LockOutEntity> LockOuts { get; set; }
    public required DbSet<UserRoleEntity> UserRoles { get; set; }
    public required DbSet<UserAgentEntity> UserAgents { get; set; }
    public required DbSet<DeletedUserEntity> DeletedUsers { get; set; }
    public required DbSet<UserRelationEntity> UserRelations { get; set; }
    public required DbSet<FriendRequestEntity> FriendRequests { get; set; }
    public required DbSet<SSOConnectionEntity> SSOConnections { get; set; }
    public required DbSet<UserPasswordResetRequestEntity> UserPasswordResetRequests { get; set; }
    public required DbSet<UserEmailVerificationRequestEntity> UserEmailVerificationRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ImageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SessionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LockOutEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserAgentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeletedUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRelationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FriendRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SSOConnectionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserPasswordResetRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEmailAddressChangeRequestEntityConfiguration());
    }

    public async Task<bool> TryCreateAsync<T>(Func<DatabaseContext, DbSet<T>> selector, T entity, ILogger logger, CancellationToken cancellationToken) where T : class
    {
        int retryCount = 0;
    retry:
        try
        {
            using IDbContextTransaction? transaction = await Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            await selector(this).AddAsync(entity, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return true;
        }
        catch (PostgresException exception)
        {
            if (exception.IsTransient && retryCount++ < 3)
            {
                goto retry;
            }

            logger.LogError("Ran out of retries while creating entity!");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create entity");
        }

        return false;
    }
}