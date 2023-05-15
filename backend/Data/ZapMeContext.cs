using Microsoft.EntityFrameworkCore;
using ZapMe.Data.Models;

namespace ZapMe.Data;

public sealed class ZapMeContext : DbContext
{
    public ZapMeContext(DbContextOptions<ZapMeContext> options) : base(options)
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
    public required DbSet<OAuthConnectionEntity> OAuthConnections { get; set; }
    public required DbSet<PasswordResetRequestEntity> PasswordResetRequests { get; set; }
    public required DbSet<EmailVerificationRequestEntity> EmailVerificationRequests { get; set; }

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
        modelBuilder.ApplyConfiguration(new OAuthConnectionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmailAddressChangeRequestEntityConfiguration());
    }
}