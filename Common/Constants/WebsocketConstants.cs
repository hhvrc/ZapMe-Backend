namespace ZapMe.Constants;

public static class WebsocketConstants
{
    public static uint ClientMessageSizeMax => 1024 * 4; // 4 KiB
    public static uint ServerMessageSizeMax => 1024 * 16; // 16 KiB

    public static uint ClientRateLimitMessagesPerSecond => 50; // Hard limit of 50 messages per second
    public static uint ClientRateLimitMessagesPerMinute => 1000; // Hard limit of 1000 messages per minute (16.6 messages per second average)

    public static uint ClientRateLimitBytesPerSecond => 1024 * 1024 * 1; // 1 MiB/s
    public static uint ClientRateLimitBytesPerMinute => 1024 * 1024 * 60; // 60 MiB/m

    internal static int ClientTxChannelCapacity => 1000; // 1000 messages queued for sending
}
