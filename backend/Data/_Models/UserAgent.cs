using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Data.Models;

public class UserAgentEntity
{
    /// <summary>
    /// Sha256 hash of the user agent string before truncation
    /// </summary>
    public required byte[] Hash { get; set; }

    /// <summary>
    /// Length of user agent string before truncation
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Truncated useragent
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public sealed class UserAgentEntityConfiguration : IEntityTypeConfiguration<UserAgentEntity>
{
    public void Configure(EntityTypeBuilder<UserAgentEntity> builder)
    {
        builder.ToTable("userAgents");

        builder.HasKey(u => u.Hash);

        builder.Property(u => u.Hash)
            .HasColumnName("hash")
            .HasMaxLength(HashConstants.Sha256LengthBin);

        builder.Property(u => u.Length)
            .HasColumnName("length");

        builder.Property(u => u.Value)
            .HasColumnName("value")
            .HasMaxLength(UserAgentLimits.StoredLength);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");
    }
}