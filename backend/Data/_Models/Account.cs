using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Enums;

namespace ZapMe.Data.Models;

public sealed class AccountEntity
{
    public static string TableName => "accounts";
    public static string TableAccountNameIndex => "accounts_name_idx";
    public static string TableAccountEmailIndex => "accounts_email_idx";
    public static string TablePasswordResetTokenIndex => "accounts_passwordResetToken_idx";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string PasswordHash { get; set; } = null!;

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
    public UserOnlineStatus OnlineStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string OnlineStatusText { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? PasswordResetRequestedAt { get; set; }

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

    public ICollection<SessionEntity>? Sessions { get; set; }
    public ICollection<LockOutEntity>? LockOuts { get; set; }
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
    public ICollection<UserRelationEntity>? Relations { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsOutgoing { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsIncoming { get; set; }
    public ICollection<OAuthConnectionEntity>? OauthConnections { get; set; }
}

public sealed class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.ToTable(AccountEntity.TableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasMaxLength(32);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(320);

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

        builder.Property(u => u.PasswordResetToken)
            .HasColumnName("passwordResetToken")
            .HasMaxLength(128);

        builder.Property(u => u.PasswordResetRequestedAt)
            .HasColumnName("passwordResetRequestedAt");

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
            .HasDatabaseName(AccountEntity.TableAccountNameIndex)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName(AccountEntity.TableAccountEmailIndex)
            .IsUnique();

        builder.HasIndex(u => u.PasswordResetToken)
            .HasDatabaseName(AccountEntity.TablePasswordResetTokenIndex)
            .IsUnique();
    }
}