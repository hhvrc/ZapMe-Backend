using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public class TemporaryStringDataEntity
{
    public const string TableName = "tempStringData";
    public const string TableExpiresAtIndex = TableName + "_expiresAt_idx";

    /// <summary/>
    public required string Key { get; set; }

    /// <summary/>
    public required string Value { get; set; }

    /// <summary/>
    public DateTime ExpiresAt { get; set; }
}

public sealed class TemporaryStringDataEntityConfiguration : IEntityTypeConfiguration<TemporaryStringDataEntity>
{
    public void Configure(EntityTypeBuilder<TemporaryStringDataEntity> builder)
    {
        builder.ToTable(TemporaryStringDataEntity.TableName);

        builder.HasKey(si => si.Key);

        builder.Property(si => si.Key)
            .HasColumnName("key");

        builder.Property(si => si.Value)
            .HasColumnName("value");

        builder.Property(si => si.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.HasIndex(si => si.ExpiresAt)
            .HasDatabaseName(TemporaryStringDataEntity.TableExpiresAtIndex);
    }
}