using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserEmailVerificationRequestEntity
{
    public const string TableName = "emailVerificationRequest";
    public const string TableNewEmailIndex = TableName + "_newEmail_idx";
    public const string TableTokenIndex = TableName + "_tokenHash_idx";

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    /// <summary>
    /// The new email address to be verified.
    /// <para>Once the user has verified the email address, this will be stored in the <see cref="UserEntity.Email"/> property, and this record will be deleted.</para>
    /// </summary>
    public required string NewEmail { get; set; }

    /// <summary>
    /// This is a hash of the token, not the token itself.
    /// <para>Storing the token itself would be a security risk if someone got read access to the database.</para>
    /// </summary>
    public required string TokenHash { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class UserEmailAddressChangeRequestEntityConfiguration : IEntityTypeConfiguration<UserEmailVerificationRequestEntity>
{
    public void Configure(EntityTypeBuilder<UserEmailVerificationRequestEntity> builder)
    {
        builder.ToTable(UserEmailVerificationRequestEntity.TableName);

        builder.HasKey(macr => macr.UserId);

        builder.Property(macr => macr.UserId)
            .HasColumnName("userId");

        builder.Property(macr => macr.NewEmail)
            .HasColumnName("newEmail")
            .HasMaxLength(GeneralHardLimits.EmailAddressMaxLength);

        builder.Property(macr => macr.TokenHash)
            .HasColumnName("tokenHash")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(macr => macr.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasOne(macr => macr.User)
            .WithOne(u => u.EmailVerificationRequest)
            .HasForeignKey<UserEmailVerificationRequestEntity>(macr => macr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(macr => macr.NewEmail)
            .IsUnique()
            .HasDatabaseName(UserEmailVerificationRequestEntity.TableNewEmailIndex);

        builder.HasIndex(macr => macr.TokenHash)
            .HasDatabaseName(UserEmailVerificationRequestEntity.TableTokenIndex)
            .IsUnique();
    }
}