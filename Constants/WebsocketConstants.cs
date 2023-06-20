namespace ZapMe.Constants;

public static class WebsocketConstants
{
    public static int ClientMessageSizeMax => 1024 * 4; // 4 KiB
    public static int ServerMessageSizeMax => 1024 * 16; // 16 KiB

    public static ulong ClientRateLimitMessagesPerSecond => 50; // Hard limit of 50 messages per second
    public static ulong ClientRateLimitMessagesPerMinute => 1000; // Hard limit of 1000 messages per minute (16.6 messages per second average)

    public static ulong ClientRateLimitBytesPerSecond => 1024 * 1024 * 1; // 1 MiB/s
    public static ulong ClientRateLimitBytesPerMinute => 1024 * 1024 * 60; // 60 MiB/m
}
