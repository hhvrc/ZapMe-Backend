using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class DeviceEntity
{
    public Guid Id { get; private set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; init; }
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