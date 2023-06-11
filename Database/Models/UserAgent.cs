using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserAgentEntity
{
    public const string TableName = "userAgents";
    public const string TableHashIndex = TableName + "_hash_idx";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Sha256 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint Length { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string OperatingSystem { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Device { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Browser { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ICollection<SessionEntity>? Sessions { get; set; }
}

public sealed class UserAgentEntityConfiguration : IEntityTypeConfiguration<UserAgentEntity>
{
    public void Configure(EntityTypeBuilder<UserAgentEntity> builder)
    {
        builder.ToTable(UserAgentEntity.TableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.Sha256)
            .HasColumnName("sha256")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(u => u.Length)
            .HasColumnName("length");

        builder.Property(u => u.Value)
            .HasColumnName("value")
            .HasMaxLength(UserAgentLimits.StoredValueLength);

        builder.Property(u => u.OperatingSystem)
            .HasColumnName("operatingSystem")
            .HasMaxLength(UserAgentLimits.StoredOperatingSystemLength);

        builder.Property(u => u.Device)
            .HasColumnName("device")
            .HasMaxLength(UserAgentLimits.StoredDeviceLength);

        builder.Property(u => u.Browser)
            .HasColumnName("browser")
            .HasMaxLength(UserAgentLimits.StoredBrowserLength);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasIndex(u => u.Sha256)
            .HasDatabaseName(UserAgentEntity.TableHashIndex)
            .IsUnique();
    }
}