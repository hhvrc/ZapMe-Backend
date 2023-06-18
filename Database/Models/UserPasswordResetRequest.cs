using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserPasswordResetRequestEntity
{
    public Guid UserId { get; set; }

    public required string TokenHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class UserPasswordResetRequestEntityConfiguration : IEntityTypeConfiguration<UserPasswordResetRequestEntity>
{
    public void Configure(EntityTypeBuilder<UserPasswordResetRequestEntity> builder)
    {
        builder.HasKey(pr => pr.UserId);
        builder.Property(pr => pr.TokenHash).HasMaxLength(HashConstants.Sha256LengthHex);
        builder.Property(pr => pr.CreatedAt).HasDefaultValueSql("now()");
        builder.HasIndex(pr => pr.TokenHash).IsUnique();
    }
}