using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class UserRoleEntity
{
    public Guid UserId { get; init; }

    public required string RoleName { get; init; }

    public DateTime CreatedAt { get; init; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleName });
        builder.Property(ur => ur.RoleName).HasMaxLength(32);
        builder.Property(ur => ur.CreatedAt).HasDefaultValueSql("now()");
        builder.HasIndex(ur => ur.UserId);
        builder.HasIndex(ur => ur.RoleName);
    }
}