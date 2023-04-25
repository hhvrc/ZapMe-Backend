using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;
using ZapMe.Enums;

namespace ZapMe.Data.Models;

public sealed class UserEntity
{
    public const string TableName = "users";
    public const string TableAccountNameIndex = TableName + "_name_idx";
    public const string TableAccountEmailIndex = TableName + "_email_idx";

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
    /// Secure hash of the user's password, hashed with <see cref="Utils.PasswordUtils"/>.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int AcceptedTosVersion { get; set; }

    /// <inheritdoc/>
    public Guid ProfilePictureId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ImageEntity? ProfilePicture { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserStatus OnlineStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string OnlineStatusText { get; set; }

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

    public EmailVerificationRequestEntity? EmailVerificationRequest { get; set; }

    public ICollection<SessionEntity>? Sessions { get; set; }
    public ICollection<LockOutEntity>? LockOuts { get; set; }
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
    public ICollection<UserRelationEntity>? Relations { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsOutgoing { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsIncoming { get; set; }
    public ICollection<OAuthConnectionEntity>? OauthConnections { get; set; }
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

        builder.Property(u => u.AcceptedTosVersion)
            .HasColumnName("acceptedTosVersion");

        builder.Property(u => u.ProfilePictureId)
            .HasColumnName("profilePictureId");

        builder.Property(u => u.OnlineStatus)
            .HasColumnName("statusOnline");

        builder.Property(u => u.OnlineStatusText)
            .HasColumnName("statusText")
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
            .HasDatabaseName(UserEntity.TableAccountNameIndex)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName(UserEntity.TableAccountEmailIndex)
            .IsUnique();
    }
}