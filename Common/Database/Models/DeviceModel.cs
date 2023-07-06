using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Enums.Devices;

namespace ZapMe.Database.Models;

public sealed class DeviceModelEntity
{
    public Guid Id { get; private set; }

    /// <summary>
    /// This doesn't need to be the name provided by the manufacturer, it is meant to be a recognizable name for the device model.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The model number of this device that was provided by the manufacturer.
    /// </summary>
    public required string ModelNumber { get; set; }

    /// <summary>
    /// A URL to the manufacturer's website for this device model.
    /// <para>If this is a nobrand, custom, or 3rd party device, this can be a link to the product page on the retailer's website.</para>
    /// </summary>
    public required string WebsiteUrl { get; set; }

    public Guid IconId { get; init; }

    public Guid ManufacturerId { get; init; }

    /// <summary>
    /// The FCC ID of this device model, if applicable.
    /// <para>This makes it easier to find the device model on the FCC website for Reverse Engineering purposes.</para>
    /// </summary>
    public string? FccId { get; init; }

    /// <summary>
    /// Indicates whether this device model has documentation available.
    /// <para>If <see langword="true"/>, the documentation is available at /docs/{id} where {id} is the ID of this device model.</para>
    /// </summary>
    public bool HasDocumentation { get; set; }

    /// <summary>
    /// The protocol used by this device model, this might be Raw RF, Bluetooth Low Energy, WiFi, etc.
    /// </summary>
    public RfProtocol Protocol { get; set; }

    /// <summary>
    /// Use Marten to access highly dynamic JSONB data for device specifications stored in <see cref="Documents.DeviceSpecification"/>, the document ID is the same as this property.
    /// <para>Use <see cref="Protocol"/> to determine which document type to use.</para>
    /// </summary>
    public Guid? SpecificationId { get; init; }

    public DateTime CreatedAt { get; init; }

    public ImageEntity? Icon { get; private set; } = null;

    public DeviceManufacturerEntity? Manufacturer { get; private set; } = null;
}

public sealed class DeviceModelEntityConfiguration : IEntityTypeConfiguration<DeviceModelEntity>
{
    public void Configure(EntityTypeBuilder<DeviceModelEntity> builder)
    {
        builder.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(i => i.Name).HasMaxLength(64);
        builder.Property(i => i.ModelNumber).HasMaxLength(64);
        builder.Property(i => i.WebsiteUrl).HasMaxLength(256);
        builder.Property(i => i.Protocol).HasConversion(v => v.ToString(), v => Enum.Parse<RfProtocol>(v));
        builder.Property(i => i.FccId).HasMaxLength(32);
        builder.Property(i => i.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(i => i.Name);
        builder.HasIndex(i => new { i.ModelNumber, i.ManufacturerId }).IsUnique();
    }
}