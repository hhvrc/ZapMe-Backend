using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class PasswordResetRequestEntity
{
    public static string TableName => "passwordResetRequests";

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
    public string Token { get; set; } = null!;

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

        builder.Property(pr => pr.Token)
            .HasColumnName("token")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne(pr => pr.Account)
            .WithOne()
            .HasForeignKey<PasswordResetRequestEntity>(pr => pr.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}