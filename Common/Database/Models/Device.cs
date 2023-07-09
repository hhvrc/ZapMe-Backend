using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class DeviceEntity
{
    public Guid Id { get; private set; }

    public Guid ModelId { get; private set; }

    public Guid OwnerId { get; private set; }

    public required string Name { get; set; }

    public Guid? IconId { get; set; }

    /// <summary>
    /// Access key for the device. Used to authenticate the device with the WebSocket endpoint.
    /// </summary>
    public required string AccessToken { get; set; }

    public DateTime CreatedAt { get; private set; }

    public DeviceModelEntity? Model { get; private set; }

    public UserEntity? Owner { get; private set; }

    public ImageEntity? Icon { get; private set; }
}

public sealed class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> builder)
    {
        builder.Property(ud => ud.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(ud => ud.Name).HasMaxLength(GeneralHardLimits.UserDeviceNameMaxLength);
        builder.Property(ud => ud.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(ud => ud.OwnerId);
        builder.HasIndex(ud => ud.AccessToken).IsUnique();
    }
}