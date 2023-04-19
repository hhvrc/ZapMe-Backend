using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Data.Models;

public sealed class MailAddressChangeRequestEntity
{
    public const string TableName = "mailAddressVerificationRequest";
    public const string TableTokenIndex = TableName + "_tokenHash_idx";

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
    public required string NewEmail { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string TokenHash { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public sealed class MailAddressChangeRequestEntityConfiguration : IEntityTypeConfiguration<MailAddressChangeRequestEntity>
{
    public void Configure(EntityTypeBuilder<MailAddressChangeRequestEntity> builder)
    {
        builder.ToTable(MailAddressChangeRequestEntity.TableName);

        builder.HasKey(pr => pr.UserId);

        builder.Property(pr => pr.UserId)
            .HasColumnName("userId");

        builder.Property(pr => pr.NewEmail)
            .HasColumnName("newEmail")
            .HasMaxLength(GeneralHardLimits.EmailAddressMaxLength);

        builder.Property(pr => pr.TokenHash)
            .HasColumnName("tokenHash")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasOne(pr => pr.User)
            .WithOne()
            .HasForeignKey<MailAddressChangeRequestEntity>(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.TokenHash)
            .HasDatabaseName(MailAddressChangeRequestEntity.TableTokenIndex)
            .IsUnique();
    }
}