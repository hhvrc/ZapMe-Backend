using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class DeviceEntity
{
    public Guid Id { get; private set; }

    public required string Name { get; set; }

    public Guid IconId { get; set; }

    public Guid ModelId { get; init; }

    public DateTime CreatedAt { get; private set; }

    public ImageEntity? Icon { get; private set; }

    public DeviceModelEntity? Model { get; private set; }
}

public sealed class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> builder)
    {
        builder.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(i => i.Name).HasMaxLength(64);
        builder.Property(i => i.CreatedAt).HasDefaultValueSql("now()");
    }
}