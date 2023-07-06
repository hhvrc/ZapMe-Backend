using Marten.Schema;

namespace ZapMe.Database.Documents;

/// <summary>
/// The Proprietary RF Protocol specification for a device, used to configure the RF transceiver.
/// </summary>
[DocumentAlias("device_proprietary_spec")]
public sealed class DeviceProprietarySpecification
{
    public Guid Id { get; init; }

    // Carrier wave modulation schemes
    public enum ModulationType
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
    public enum EncodingType
    {
        None,
        Proprietary,
        Manchester,
    }

    public ulong Frequency { get; init; }

    public ModulationType Modulation { get; init; }

    public ulong DataRate { get; init; }

    public ulong PacketSize { get; init; }

    public EncodingType Encoding { get; init; }

    public bool InvertedBits { get; init; }
}