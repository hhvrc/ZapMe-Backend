using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

/// <summary>
/// Represents a database table for collecting anonymized feedback on account deletion actions, providing valuable insights for improvement and ensuring accountability.
/// <para>The table will also be used to retain references to user data like chat history and uploaded files, enabling gradual deletion to minimize peak loads onto infrastructure.</para>
/// </summary>
public class DeletedUserEntity
{
    public const string TableName = "deletedUsers";

    public Guid Id { get; set; }

    /// <summary>
    /// Represents the identifier of the user who performed the deletion, whether it's a moderator or the user themselves.
    /// </summary>
    public Guid DeletedBy { get; set; }

    /// <summary>
    /// Indicates the reason provided by the moderator for the deletion, or the reason given by the user for deleting their account.
    /// </summary>
    public string? DeletionReason { get; set; }

    /// <summary>
    /// Represents the timestamp when the user account was created.
    /// </summary>
    public DateTime UserCreatedAt { get; set; }

    /// <summary>
    /// Represents the timestamp when the user account was deleted.
    /// </summary>
    public DateTime UserDeletedAt { get; set; }
}

public sealed class DeletedUserEntityConfiguration : IEntityTypeConfiguration<DeletedUserEntity>
{
    public void Configure(EntityTypeBuilder<DeletedUserEntity> builder)
    {
        builder.ToTable(DeletedUserEntity.TableName);

        builder.HasKey(du => du.Id);

        builder.Property(ur => ur.Id)
            .HasColumnName("id");

        builder.Property(ur => ur.DeletedBy)
            .HasColumnName("deletedBy");

        builder.Property(ur => ur.DeletionReason)
            .HasColumnName("deletionReason")
            .HasMaxLength(256);

        builder.Property(du => du.UserCreatedAt)
            .HasColumnName("userCreatedAt");

        builder.Property(du => du.UserDeletedAt)
            .HasColumnName("userDeletedAt")
            .HasDefaultValueSql("now()");
    }
}
