using Microsoft.EntityFrameworkCore;
using ZapMe.Database.Models;

namespace ZapMe.Database;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public required DbSet<UserEntity> Users { get; set; }
    public required DbSet<ImageEntity> Images { get; set; }
    public required DbSet<DeviceEntity> Devices { get; set; }
    public required DbSet<SessionEntity> Sessions { get; set; }
    public required DbSet<LockOutEntity> LockOuts { get; set; }
    public required DbSet<UserRoleEntity> UserRoles { get; set; }
    public required DbSet<UserAgentEntity> UserAgents { get; set; }
    public required DbSet<DeletedUserEntity> DeletedUsers { get; set; }
    public required DbSet<UserRelationEntity> UserRelations { get; set; }
    public required DbSet<FriendRequestEntity> FriendRequests { get; set; }
    public required DbSet<SSOConnectionEntity> SSOConnections { get; set; }
    public required DbSet<PrivacyPolicyDocumentEntity> PrivacyPolicyDocuments { get; set; }
    public required DbSet<TermsOfServiceDocumentEntity> TermsOfServiceDocuments { get; set; }
    public required DbSet<UserPasswordResetRequestEntity> UserPasswordResetRequests { get; set; }
    public required DbSet<UserEmailVerificationRequestEntity> UserEmailVerificationRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ImageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SessionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LockOutEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserAgentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeletedUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRelationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FriendRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SSOConnectionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PrivacyPolicyDocumentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TermsOfServiceDocumentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserPasswordResetRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEmailAddressChangeRequestEntityConfiguration());
    }
}