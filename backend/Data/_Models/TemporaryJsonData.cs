using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZapMe.Data.Models;

public class TemporaryJsonDataEntity
{
    public const string TableName = "tempJsonData";
    public const string TableExpiresAtIndex = TableName + "_expiresAt_idx";

    /// <summary/>
    public required string Key { get; set; }

    /// <summary/>
    [Column(TypeName = "jsonb")]
    public required string Value { get; set; }

    /// <summary/>
    public DateTime ExpiresAt { get; set; }
}

public sealed class TemporaryJsonDataEntityConfiguration : IEntityTypeConfiguration<TemporaryJsonDataEntity>
{
    public void Configure(EntityTypeBuilder<TemporaryJsonDataEntity> builder)
    {
        builder.ToTable(TemporaryJsonDataEntity.TableName);

        builder.HasKey(si => si.Key);

        builder.Property(si => si.Key)
            .HasColumnName("key");

        builder.Property(si => si.Value)
            .HasColumnName("value");

        builder.Property(si => si.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.HasIndex(si => si.ExpiresAt)
            .HasDatabaseName(TemporaryJsonDataEntity.TableExpiresAtIndex);
    }
}