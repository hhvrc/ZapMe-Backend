namespace ZapMe.Enums.Devices;

// High level RF protocols
public enum RfProtocol
{
    Proprietary, // Proprietary protocol, needs reverse engineering by means of RTL-SDR, HackRF, YardStick One, etc.
    BLE,         // Future, to be implemented
    WiFi,        // Future, to be implemented
    ZigBee,      // Future, to be implemented (If needed, I don't think any device uses ZigBee)
}