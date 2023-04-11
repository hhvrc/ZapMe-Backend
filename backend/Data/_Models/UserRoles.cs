using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class UserRoleEntity
{
    public const string TableName = "userRoles";
    public const string TableUserIdIndex = TableName + "_userId_idx";
    public const string TableRoleNameIndex = TableName + "_roleName_idx";

    /// <summary>
    /// 
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required AccountEntity User { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string RoleName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable(UserRoleEntity.TableName);

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
            .HasDatabaseName(UserRoleEntity.TableUserIdIndex);

        builder.HasIndex(ur => ur.RoleName)
            .HasDatabaseName(UserRoleEntity.TableRoleNameIndex);
    }
}