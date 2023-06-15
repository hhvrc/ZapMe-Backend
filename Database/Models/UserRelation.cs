using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;
using ZapMe.Enums;

namespace ZapMe.Database.Models;

public sealed class UserRelationEntity
{
    public const string TableName = "userRelations";

    public Guid SourceUserId { get; set; }

    public Guid TargetUserId { get; set; }

    public UserRelationType RelationType { get; set; }

    public string? NickName { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserEntity SourceUser { get; private set; } = null!;
    public UserEntity TargetUser { get; private set; } = null!;
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
            .HasMaxLength(GeneralHardLimits.NickNameMaxLength);

        builder.Property(ur => ur.Notes)
            .HasColumnName("notes")
            .HasMaxLength(GeneralHardLimits.NotesMaxLength);

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