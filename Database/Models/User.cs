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
    public required string Email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool EmailVerified { get; set; }

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
    public Guid? ProfilePictureId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? ProfileBannerId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserPresence Presence { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string StatusMessage { get; set; }

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

    public ImageEntity? ProfilePicture { get; set; }
    public ImageEntity? ProfileBanner { get; set; }
    public UserPasswordResetRequestEntity? PasswordResetRequest { get; set; }
    public UserEmailVerificationRequestEntity? EmailVerificationRequest { get; set; }

    public ICollection<SessionEntity>? Sessions { get; set; }
    public ICollection<LockOutEntity>? LockOuts { get; set; }
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
    public ICollection<UserRelationEntity>? Relations { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsOutgoing { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsIncoming { get; set; }
    public ICollection<SSOConnectionEntity>? SSOConnections { get; set; }
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

        builder.Property(u => u.EmailVerified)
            .HasColumnName("emailVerified");

        builder.Property(u => u.PasswordHash)
            .HasColumnName("passwordHash")
            .HasMaxLength(120);

        builder.Property(u => u.AcceptedPrivacyPolicyVersion)
            .HasColumnName("acceptedPrivacyPolicyVersion");

        builder.Property(u => u.AcceptedTermsOfServiceVersion)
            .HasColumnName("acceptedTermsOfServiceVersion");

        builder.Property(u => u.ProfilePictureId)
            .HasColumnName("profilePictureId");

        builder.Property(u => u.ProfileBannerId)
            .HasColumnName("profileBannerId");

        builder.Property(u => u.Presence)
            .HasColumnName("presence");

        builder.Property(u => u.StatusMessage)
            .HasColumnName("statusMessage")
            .HasMaxLength(128);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updatedAt")
            .HasDefaultValueSql("now()");

        builder.Property(u => u.LastOnline)
            .HasColumnName("lastOnline")
            .HasDefaultValueSql("now()");

        builder.HasOne(u => u.ProfilePicture)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.UserRoles)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Name)
            .HasDatabaseName(UserEntity.TableUserNameIndex)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName(UserEntity.TableUserEmailIndex)
            .IsUnique();
    }
}