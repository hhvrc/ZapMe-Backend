using Marten.Schema;

namespace ZapMe.Database.Documents;

// Carrier wave modulation schemes
public enum RfRawModulation
{
    ASK,   // Amplitude Shift Keying
    OOK,   // On-Off Keying
    GFSK,  // Gaussian Frequency Shift Keying
    FSK,   // Frequency Shift Keying
    FSK2,  // Frequency Shift Keying 2
    FSK4,  // Frequency Shift Keying 4
    MSK,   // Minimum Shift Keying
}

// Bit-level encoding schemes
public enum RfRawEncoding
{
    None,
    Proprietary,
    Manchester,
}

public enum RfMessagePartRole
{
    Padding,
    Preamble,
    SyncWord,
    PayloadDeviceId,
    PayloadParameter,
    PayloadChannel,
    PayloadChecksum,
}

public sealed class MessagePart
{
    public RfMessagePartRole Role { get; init; }

    public string? ParameterName { get; init; }

    public uint LengthBits { get; init; }

    public string? ValueHex { get; init; }
}

public sealed class RfRawSpec
{
    public ulong Frequency { get; init; }

    public RfRawModulation Modulation { get; init; }

    public ulong DataRate { get; init; }

    public ulong PacketSize { get; init; }

    public RfRawEncoding Encoding { get; init; }

    public bool Inverted { get; init; }

    public required MessagePart[] MessageSpec { get; init; }
}

public sealed class RfBleSpec
{

}

public sealed class RfWiFiSpec
{

}

[DocumentAlias("device_spec")]
public sealed class DeviceSpecification
{
    public Guid Id { get; init; }

    public RfRawSpec? RawSpec { get; init; }

    public RfBleSpec? BleSpec { get; init; }   // Future, to be implemented

    public RfWiFiSpec? WiFiSpec { get; init; } // Future, to be implemented
}
