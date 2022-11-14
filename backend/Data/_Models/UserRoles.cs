using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class UserRoleEntity
{
    public Guid UserId { get; set; }
    public required UserEntity User { get; set; }

    public required string RoleName { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable("userRoles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleName });

        builder.Property(ur => ur.UserId)
            .HasColumnName("userId");

        builder.Property(ur => ur.RoleName)
            .HasColumnName("roleName")
            .HasMaxLength(32);

        builder.Property(ur => ur.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("userRoles_userId_idx");

        builder.HasIndex(ur => ur.RoleName)
            .HasDatabaseName("userRoles_roleName_idx");
    }
}