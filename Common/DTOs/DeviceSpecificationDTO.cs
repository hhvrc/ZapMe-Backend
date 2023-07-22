using ZapMe.Database.Documents;

namespace ZapMe.DTOs;

public readonly struct DeviceSpecificationDto
{
    public Guid Id { get; init; }

    public ulong Frequency { get; init; }

    public DeviceProprietarySpecification.ModulationType Modulation { get; init; }

    public ulong DataRate { get; init; }

    public ulong PacketSize { get; init; }

    public DeviceProprietarySpecification.EncodingType Encoding { get; init; }

    public bool InvertedBits { get; init; }
}