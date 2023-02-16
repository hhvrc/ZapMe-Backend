using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public enum UserRelationType
{
    None,
    Friend,
    Blocked
}

public sealed class UserRelationEntity
{
    public static string TableName => "userRelations";

    /// <summary>
    /// 
    /// </summary>
    public Guid SourceUserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required AccountEntity SourceUser { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid TargetUserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required AccountEntity TargetUser { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserRelationType RelationType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public sealed class UserRelationEntityConfiguration : IEntityTypeConfiguration<UserRelationEntity>
{
    public void Configure(EntityTypeBuilder<UserRelationEntity> builder)
    {
        builder.ToTable(UserRelationEntity.TableName);

        builder.HasKey(ur => new { ur.SourceUserId, ur.TargetUserId });

        builder.Property(ur => ur.SourceUserId)
            .HasColumnName("sourceUserId");

        builder.Property(ur => ur.TargetUserId)
            .HasColumnName("targetUserId");

        builder.Property(ur => ur.RelationType)
            .HasColumnName("relationType");

        builder.Property(ur => ur.NickName)
            .HasColumnName("nickName")
            .HasMaxLength(32);

        builder.Property(ur => ur.Notes)
            .HasColumnName("notes")
            .HasMaxLength(128);

        builder.Property(ur => ur.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasOne(ur => ur.SourceUser)
            .WithMany(u => u.Relations)
            .HasForeignKey(ur => ur.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.TargetUser)
            .WithMany()
            .HasForeignKey(ur => ur.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}