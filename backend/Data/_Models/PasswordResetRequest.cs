using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class PasswordResetRequestEntity
{
    public const string TableName = "passwordResetRequests";
    public const string TableTokenIndex = TableName + "_tokenHash_idx";

    /// <summary>
    /// 
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required AccountEntity Account { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public sealed class PasswordResetRequestEntityConfiguration : IEntityTypeConfiguration<PasswordResetRequestEntity>
{
    public void Configure(EntityTypeBuilder<PasswordResetRequestEntity> builder)
    {
        builder.ToTable(PasswordResetRequestEntity.TableName);

        builder.HasKey(pr => pr.AccountId);

        builder.Property(pr => pr.AccountId)
            .HasColumnName("accountId")
            .IsRequired();

        builder.Property(pr => pr.TokenHash)
            .HasColumnName("tokenHash")
            .HasMaxLength(32) // Sha256 hash digest
            .IsRequired();

        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne(pr => pr.Account)
            .WithOne()
            .HasForeignKey<PasswordResetRequestEntity>(pr => pr.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.TokenHash)
            .HasDatabaseName(PasswordResetRequestEntity.TableTokenIndex)
            .IsUnique();
    }
}