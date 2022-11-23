using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Records;

namespace ZapMe.Data.Models;

public sealed class AccountEntity : IUserRecord
{
    /// <inheritdoc/>
    public Guid Id { get; set; }

    /// <inheritdoc/>
    public string UserName { get; set; } = null!;

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

    /// <inheritdoc/>
    public UserOnlineStatus OnlineStatus { get; set; }

    /// <inheritdoc/>
    public string OnlineStatusText { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? PasswordResetRequestedAt { get; set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <inheritdoc/>
    public DateTime LastOnline { get; set; }

    public ICollection<SignInEntity>? SignIns { get; set; }
    public ICollection<LockOutEntity>? LockOuts { get; set; }
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
    public ICollection<UserRelationEntity>? Relations { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsOutgoing { get; set; }
    public ICollection<FriendRequestEntity>? FriendRequestsIncoming { get; set; }
    public ICollection<OAuthConnectionEntity>? OauthConnections { get; set; }
}

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.UserName)
            .HasColumnName("userName")
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

        builder.HasIndex(u => u.UserName)
            .HasDatabaseName("users_username_idx")
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName("users_email_idx")
            .IsUnique();

        builder.HasIndex(u => u.PasswordResetToken)
            .HasDatabaseName("users_passwordResetToken_idx")
            .IsUnique();
    }
}