using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;
using ZapMe.Enums;

namespace ZapMe.Database.Models;

public sealed class UserEntity
{
    public const string TableName = "users";
    public const string TableUserNameIndex = TableName + "_name_idx";
    public const string TableUserEmailIndex = TableName + "_email_idx";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Username, also referred to as display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Secure hash of the user's password, hashed with BCrypt.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint AcceptedPrivacyPolicyVersion { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint AcceptedTermsOfServiceVersion { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? ProfileAvatarId { get; set; }
    public ImageEntity? ProfileAvatar { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? ProfileBannerId { get; set; }
    public ImageEntity? ProfileBanner { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string StatusText { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    public DateTime LastOnline { get; set; }

    public UserPasswordResetRequestEntity? PasswordResetRequest { get; set; }
    public UserEmailVerificationRequestEntity? EmailVerificationRequest { get; set; }

    public ICollection<SessionEntity> Sessions { get; set; } = null!;
    public ICollection<LockOutEntity> LockOuts { get; set; } = null!;
    public ICollection<UserRoleEntity> UserRoles { get; set; } = null!;
    public ICollection<UserRelationEntity> Relations { get; set; } = null!;
    public ICollection<FriendRequestEntity> FriendRequestsOutgoing { get; set; } = null!;
    public ICollection<FriendRequestEntity> FriendRequestsIncoming { get; set; } = null!;
    public ICollection<SSOConnectionEntity> SSOConnections { get; set; } = null!;
}

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(UserEntity.TableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasMaxLength(GeneralHardLimits.UsernameMaxLength);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(GeneralHardLimits.EmailAddressMaxLength);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("passwordHash")
            .HasMaxLength(HashConstants.BCryptHashLength);

        builder.Property(u => u.AcceptedPrivacyPolicyVersion)
            .HasColumnName("acceptedPrivacyPolicyVersion");

        builder.Property(u => u.AcceptedTermsOfServiceVersion)
            .HasColumnName("acceptedTermsOfServiceVersion");

        builder.Property(u => u.ProfileAvatarId)
            .HasColumnName("profileAvatarId");

        builder.Property(u => u.ProfileBannerId)
            .HasColumnName("profileBannerId");

        builder.Property(u => u.Status)
            .HasColumnName("status");

        builder.Property(u => u.StatusText)
            .HasColumnName("statusText")
            .HasMaxLength(GeneralHardLimits.StatusTextMaxLength);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updatedAt")
            .HasDefaultValueSql("now()");

        builder.Property(u => u.LastOnline)
            .HasColumnName("lastOnline")
            .HasDefaultValueSql("now()");

        builder.HasOne(u => u.ProfileAvatar)
            .WithMany()
            .HasForeignKey(u => u.ProfileAvatarId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.ProfileBanner)
            .WithMany()
            .HasForeignKey(u => u.ProfileBannerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(u => u.Name)
            .HasDatabaseName(UserEntity.TableUserNameIndex)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName(UserEntity.TableUserEmailIndex)
            .IsUnique();
    }
}