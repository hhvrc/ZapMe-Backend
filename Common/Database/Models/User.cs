using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;
using ZapMe.Enums;

namespace ZapMe.Database.Models;

public sealed class UserEntity
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Username, also referred to as display name.
    /// </summary>
    public required string Name { get; set; }

    public required string Email { get; set; }

    public bool EmailVerified { get; set; }

    /// <summary>
    /// Secure hash of the user's password, hashed with BCrypt.
    /// </summary>
    public required string PasswordHash { get; set; }

    public uint AcceptedPrivacyPolicyVersion { get; set; }

    public uint AcceptedTermsOfServiceVersion { get; set; }

    public Guid? ProfileAvatarId { get; set; }

    public Guid? ProfileBannerId { get; set; }

    public UserStatus Status { get; set; }

    public required string StatusText { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    public DateTime LastOnline { get; set; }

    public ImageEntity? ProfileAvatar { get; private set; } = null;
    public ImageEntity? ProfileBanner { get; private set; } = null;
    public UserPasswordResetRequestEntity? PasswordResetRequest { get; private set; } = null;
    public UserEmailVerificationRequestEntity? EmailVerificationRequest { get; private set; } = null;
    public List<SessionEntity> Sessions { get; private set; } = new List<SessionEntity>();
    public List<LockOutEntity> LockOuts { get; private set; } = new List<LockOutEntity>();
    public List<UserRoleEntity> UserRoles { get; private set; } = new List<UserRoleEntity>();
    public List<UserRelationEntity> RelationsOutgoing { get; private set; } = new List<UserRelationEntity>();
    public List<UserRelationEntity> RelationsIncoming { get; private set; } = new List<UserRelationEntity>();
    public List<SSOConnectionEntity> SSOConnections { get; private set; } = new List<SSOConnectionEntity>();
}

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.Name).HasMaxLength(GeneralHardLimits.UsernameMaxLength);
        builder.Property(u => u.Email).HasMaxLength(GeneralHardLimits.EmailAddressMaxLength);
        builder.Property(u => u.PasswordHash).HasMaxLength(HashConstants.BCryptHashLength);
        builder.Property(u => u.StatusText).HasMaxLength(GeneralHardLimits.StatusTextMaxLength);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(u => u.UpdatedAt).HasDefaultValueSql("now()");
        builder.Property(u => u.LastOnline).HasDefaultValueSql("now()");
        builder.HasIndex(u => u.Name).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(u => u.ProfileAvatar).WithMany().OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(u => u.ProfileBanner).WithMany().OnDelete(DeleteBehavior.SetNull);
    }
}