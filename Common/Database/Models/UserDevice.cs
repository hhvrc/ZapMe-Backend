using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserDeviceEntity
{
    public Guid UserId { get; private set; }

    public Guid DeviceId { get; private set; }

    public required string Name { get; set; }

    /// <summary>
    /// Access key for the device. Used to authenticate the device with the WebSocket endpoint.
    /// </summary>
    public required string AccessToken { get; set; }

    public Guid? IconId { get; set; }

    public DateTime CreatedAt { get; private set; }

    public UserEntity? User { get; private set; }

    public ImageEntity? Icon { get; private set; }
}

public sealed class UserDeviceEntityConfiguration : IEntityTypeConfiguration<UserDeviceEntity>
{
    public void Configure(EntityTypeBuilder<UserDeviceEntity> builder)
    {
        builder.HasKey(ud => new { ud.UserId, ud.DeviceId });
        builder.Property(ud => ud.Name).HasMaxLength(GeneralHardLimits.UserDeviceNameMaxLength);

        builder.HasIndex(ud => ud.UserId);
        builder.HasIndex(ud => ud.DeviceId).IsUnique();
    }
}