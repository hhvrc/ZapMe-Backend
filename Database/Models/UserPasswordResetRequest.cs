using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserPasswordResetRequestEntity
{
    public const string TableName = "userPasswordResetRequests";
    public const string TableTokenIndex = TableName + "_tokenHash_idx";

    public Guid UserId { get; set; }

    public required string TokenHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class UserPasswordResetRequestEntityConfiguration : IEntityTypeConfiguration<UserPasswordResetRequestEntity>
{
    public void Configure(EntityTypeBuilder<UserPasswordResetRequestEntity> builder)
    {
        builder.ToTable(UserPasswordResetRequestEntity.TableName);

        builder.HasKey(pr => pr.UserId);

        builder.Property(pr => pr.UserId)
            .HasColumnName("userId");

        builder.Property(pr => pr.TokenHash)
            .HasColumnName("tokenHash")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasOne(pr => pr.User)
            .WithOne(u => u.PasswordResetRequest)
            .HasForeignKey<UserPasswordResetRequestEntity>(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.TokenHash)
            .HasDatabaseName(UserPasswordResetRequestEntity.TableTokenIndex)
            .IsUnique();
    }
}