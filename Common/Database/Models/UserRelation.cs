using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;
using ZapMe.Enums;

namespace ZapMe.Database.Models;

public sealed class UserRelationEntity
{
    public Guid FromUserId { get; init; }

    public Guid ToUserId { get; init; }

    public UserPartialFriendStatus FriendStatus { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsMuted { get; set; }

    public string NickName { get; set; } = String.Empty;

    public string Notes { get; set; } = String.Empty;

    public DateTime CreatedAt { get; init; }

    public UserEntity SourceUser { get; private set; } = null!;
    public UserEntity TargetUser { get; private set; } = null!;
}

public sealed class UserRelationEntityConfiguration : IEntityTypeConfiguration<UserRelationEntity>
{
    public void Configure(EntityTypeBuilder<UserRelationEntity> builder)
    {
        builder.HasKey(ur => new { ur.FromUserId, ur.ToUserId });
        builder.Property(ur => ur.NickName).HasMaxLength(GeneralHardLimits.NickNameMaxLength);
        builder.Property(ur => ur.Notes).HasMaxLength(GeneralHardLimits.NotesMaxLength);
        builder.Property(ur => ur.CreatedAt).HasDefaultValueSql("now()");

        builder.HasOne(ur => ur.SourceUser)
            .WithMany(u => u.RelationsOutgoing)
            .HasForeignKey(ur => ur.FromUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.TargetUser)
            .WithMany(u => u.RelationsIncoming)
            .HasForeignKey(ur => ur.ToUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}