using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserEmailVerificationRequestEntity
{
    public Guid UserId { get; init; }

    /// <summary>
    /// The new email address to be verified.
    /// <para>Once the user has verified the email address, this will be stored in the <see cref="UserEntity.Email"/> property, and this record will be deleted.</para>
    /// </summary>
    public required string NewEmail { get; init; }

    /// <summary>
    /// This is a hash of the token, not the token itself.
    /// <para>Storing the token itself would be a security risk if someone got read access to the database.</para>
    /// </summary>
    public required string TokenHash { get; init; }

    public DateTime CreatedAt { get; init; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class UserEmailAddressChangeRequestEntityConfiguration : IEntityTypeConfiguration<UserEmailVerificationRequestEntity>
{
    public void Configure(EntityTypeBuilder<UserEmailVerificationRequestEntity> builder)
    {
        builder.HasKey(macr => macr.UserId);
        builder.Property(macr => macr.NewEmail).HasMaxLength(GeneralHardLimits.EmailAddressMaxLength);
        builder.Property(macr => macr.TokenHash).HasMaxLength(HashConstants.Sha256LengthHex);
        builder.Property(macr => macr.CreatedAt).HasDefaultValueSql("now()");
        builder.HasIndex(macr => macr.NewEmail).IsUnique();
        builder.HasIndex(macr => macr.TokenHash).IsUnique();

    }
}