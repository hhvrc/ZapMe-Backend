using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class LockOutEntity
{
    public const string TableName = "lockOuts";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserEntity? User { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Flags { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}

public sealed class LockOutEntityConfiguration : IEntityTypeConfiguration<LockOutEntity>
{
    public void Configure(EntityTypeBuilder<LockOutEntity> builder)
    {
        builder.ToTable(LockOutEntity.TableName);

        builder.HasKey(lo => lo.Id);

        builder.Property(lo => lo.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(lo => lo.UserId)
            .HasColumnName("userId");

        builder.Property(lo => lo.Reason)
            .HasColumnName("reason");

        builder.Property(lo => lo.Flags)
            .HasColumnName("flags");

        builder.Property(lo => lo.CreatedAt)
            .HasColumnName("createdAt");

        builder.Property(lo => lo.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.HasOne(lo => lo.User)
            .WithMany(u => u.LockOuts)
            .HasForeignKey(lo => lo.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}