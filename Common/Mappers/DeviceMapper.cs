using ZapMe.Database.Models;

namespace ZapMe.DTOs;

public static class DeviceMapper
{
    public static DeviceDto MapToDeviceDto(DeviceEntity device)
    {
        return new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            ModelId = device.ModelId,
            ModelName = device.Model!.Name,
            ModelNumber = device.Model!.ModelNumber,
            ModelWebsiteUrl = device.Model!.WebsiteUrl,
            IconUrl = device.Model!.Icon!.PublicUrl,
            Protocol = device.Model!.Protocol,
            ManufacturerId = device.Model!.ManufacturerId,
            ManufacturerName = device.Model!.Manufacturer!.Name,
            ManufacturerWebsiteUrl = device.Model!.Manufacturer!.WebsiteUrl,
            RegisteredAt = device.CreatedAt
        };
    }
}
