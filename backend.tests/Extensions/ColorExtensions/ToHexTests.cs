using System.Drawing;

namespace ZapMe.Tests.Extensions.ColorExtensions;

public sealed class ToHexTests
{
    [Fact]
    public void Black()
    {
        Assert.Equal("#000000", Color.Black.ToHex());
    }

    [Fact]
    public void White()
    {
        Assert.Equal("#FFFFFF", Color.White.ToHex());
    }

    [Fact]
    public void Gray()
    {
        Assert.Equal("#808080", Color.Gray.ToHex());
    }

    [Fact]
    public void Transparent()
    {
        Assert.Equal("#ABCDEF00", Color.FromArgb(0x00, 0xAB, 0xCD, 0xEF).ToHex());
    }

    [Fact]
    public void SemiTransparent()
    {
        Assert.Equal("#ABCDEF80", Color.FromArgb(0x80, 0xAB, 0xCD, 0xEF).ToHex());
    }
}
