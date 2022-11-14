namespace System.Drawing;

public static class ColorExtensions
{
    public static string ToHex(this Color c) => c.A == 0xFF ? $"#{c.R:X2}{c.G:X2}{c.B:X2}" : $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";
}
