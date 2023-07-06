using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class DeviceManufacturerEntity
{
    public Guid Id { get; private set; }

    /// <summary>
    /// A callname for the manufacturer, e.g. "Samsung".
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A URL to the manufacturer's website.
    /// </summary>
    public required string WebsiteUrl { get; set; }

    public Guid IconId { get; init; }

    public DateTime CreatedAt { get; init; }

    public ImageEntity? Icon { get; private set; } = null;
}

public sealed class DeviceManufacturerEntityConfiguration : IEntityTypeConfiguration<DeviceManufacturerEntity>
{
    public void Configure(EntityTypeBuilder<DeviceManufacturerEntity> builder)
    {
        builder.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(i => i.Name).HasMaxLength(64);
        builder.Property(i => i.WebsiteUrl).HasMaxLength(256);
        builder.Property(i => i.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(i => i.Name).IsUnique();
    }
}