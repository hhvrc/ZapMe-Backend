using FlatSharp;

namespace System;

public static class ArraySegmentExtensions
{
    public static MemoryInputBuffer ToFlatSharpMemory(this ArraySegment<byte> arraySegment)
    {
        return new MemoryInputBuffer(arraySegment.AsMemory());
    }
}