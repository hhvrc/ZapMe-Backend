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
    public Guid SourceUserId { get; set; }
    public required UserEntity SourceUser { get; set; }

    public Guid TargetUserId { get; set; }
    public required UserEntity TargetUser { get; set; }

    public UserRelationType RelationType { get; set; }

    public string? NickName { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class UserRelationEntityConfiguration : IEntityTypeConfiguration<UserRelationEntity>
{
    public void Configure(EntityTypeBuilder<UserRelationEntity> builder)
    {
        builder.ToTable("userRelations");

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