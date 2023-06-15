using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class DeviceEntity
{
    public const string TableName = "devices";

    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> builder)
    {
        builder.ToTable(DeviceEntity.TableName);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .HasMaxLength(64);

        builder.Property(i => i.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");
    }
}