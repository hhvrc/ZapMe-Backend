using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public enum UserOnlineStatus
{
    Invisible,
    DoNotDisturb,
    Away,
    Online,
    DownBad
}

public sealed class UserEntity
{
    public Guid Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public bool EmailVerified { get; set; }

    public required string PasswordHash { get; set; }

    public int AcceptedTosVersion { get; set; }

    public Guid? ProfilePictureId { get; set; }
    public ImageEntity? ProfilePicture { get; set; }

    public UserOnlineStatus OnlineStatus { get; set; }
    public required string StatusText { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetRequestedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime LastOnline { get; set; }

    public ICollection<SignInEntity>? SignIns { get; set; }
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

        builder.Property(u => u.StatusText)
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