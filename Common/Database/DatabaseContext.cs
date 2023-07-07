using Microsoft.EntityFrameworkCore;
using ZapMe.Database.Models;

namespace ZapMe.Database;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    // User
    public required DbSet<UserEntity> Users { get; set; }
    public required DbSet<UserRoleEntity> UserRoles { get; set; }
    public required DbSet<UserRelationEntity> UserRelations { get; set; }
    public required DbSet<UserDeviceEntity> UserDevices { get; set; }
    public required DbSet<UserPasswordResetRequestEntity> UserPasswordResetRequests { get; set; }
    public required DbSet<UserEmailVerificationRequestEntity> UserEmailVerificationRequests { get; set; }
    public required DbSet<DeletedUserEntity> DeletedUsers { get; set; }

    // Sessions and security
    public required DbSet<SessionEntity> Sessions { get; set; }
    public required DbSet<SSOConnectionEntity> SSOConnections { get; set; }
    public required DbSet<UserAgentEntity> UserAgents { get; set; }
    public required DbSet<LockOutEntity> LockOuts { get; set; }

    // Media
    public required DbSet<ImageEntity> Images { get; set; }

    // Devices
    public required DbSet<DeviceEntity> Devices { get; set; }
    public required DbSet<DeviceModelEntity> DeviceModels { get; set; }
    public required DbSet<DeviceManufacturerEntity> DeviceManufacturers { get; set; }

    // Control grants
    public required DbSet<ControlGrantEntity> ControlGrants { get; set; }
    public required DbSet<ControlGrantUserShareEntity> ControlGrantUserShares { get; set; }
    public required DbSet<ControlGrantPublicShareEntity> ControlGrantPublicShares { get; set; }

    // Documents
    public required DbSet<PrivacyPolicyDocumentEntity> PrivacyPolicyDocuments { get; set; }
    public required DbSet<TermsOfServiceDocumentEntity> TermsOfServiceDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRelationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserDeviceEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserPasswordResetRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEmailVerificationRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeletedUserEntityConfiguration());

        modelBuilder.ApplyConfiguration(new SessionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SSOConnectionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserAgentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LockOutEntityConfiguration());

        modelBuilder.ApplyConfiguration(new ImageEntityConfiguration());

        modelBuilder.ApplyConfiguration(new DeviceEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceModelEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceManufacturerEntityConfiguration());

        modelBuilder.ApplyConfiguration(new ControlGrantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ControlGrantUserShareEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ControlGrantPublicShareEntityConfiguration());

        modelBuilder.ApplyConfiguration(new PrivacyPolicyDocumentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TermsOfServiceDocumentEntityConfiguration());
    }
}