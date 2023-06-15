using ZapMe.Database.Models;

namespace ZapMe.DTOs;

public static class DeviceMapper
{
    public static DeviceDto ToDeviceDto(this DeviceEntity user)
    {
        return new DeviceDto
        {
            Id = user.Id,
            RegisteredAt = user.CreatedAt
        };
    }
}
