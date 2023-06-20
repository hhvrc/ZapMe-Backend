using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapMe.Constants;

public static class WebsocketConstants
{
    public static int ClientMessageSizeMax => 1024 * 4; // 4 KiB
    public static int ServerMessageSizeMax => 1024 * 16; // 16 KiB

    public static ulong ClientRateLimitMessagesPerSecond => 10; // Hard limit of 10 messages per second
    public static ulong ClientRateLimitMessagesPerMinute => 600; // Hard limit of 600 messages per minute

    public static ulong ClientRateLimitBytesPerSecond => 1024 * 1024 * 1; // 1 MiB/s
    public static ulong ClientRateLimitBytesPerMinute => 1024 * 1024 * 60; // 60 MiB/m
}
