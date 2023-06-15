﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserAgentEntity
{
    public const string TableName = "userAgents";
    public const string TableHashIndex = TableName + "_hash_idx";

    public Guid Id { get; set; }

    public required string Sha256 { get; set; }

    public uint Length { get; set; }

    public required string Value { get; set; }

    public required string OperatingSystem { get; set; }

    public required string Device { get; set; }

    public required string Browser { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<SessionEntity> Sessions { get; private set; } = new List<SessionEntity>();
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