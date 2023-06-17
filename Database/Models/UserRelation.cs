using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;
using ZapMe.Enums;

namespace ZapMe.Database.Models;

public sealed class UserRelationEntity
{
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
        builder.HasKey(ur => new { ur.SourceUserId, ur.TargetUserId });
        builder.Property(ur => ur.NickName).HasMaxLength(GeneralHardLimits.NickNameMaxLength);
        builder.Property(ur => ur.Notes).HasMaxLength(GeneralHardLimits.NotesMaxLength);
        builder.Property(ur => ur.CreatedAt).HasDefaultValueSql("now()");

        builder.HasOne(ur => ur.SourceUser)
            .WithMany(u => u.RelationsOutgoing)
            .HasForeignKey(ur => ur.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.TargetUser)
            .WithMany(u => u.RelationsIncoming)
            .HasForeignKey(ur => ur.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}