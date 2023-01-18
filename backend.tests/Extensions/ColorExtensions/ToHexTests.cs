using System.Drawing;

namespace ZapMe.Tests.Extensions.ColorExtensions;

public sealed class ToHexTests
{
    [Fact]
    public void ToHex_Black_ReturnsAllZero()
    {
        // Arrange
        Color color = Color.Black;

        // Act
        string hex = color.ToHex();

        // Assert
        Assert.Equal("#000000", hex);
    }

    [Fact]
    public void ToHex_Black_ReturnsAllF()
    {
        // Arrange
        Color color = Color.White;

        // Act
        string hex = color.ToHex();

        // Assert
        Assert.Equal("#FFFFFF", hex);
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
