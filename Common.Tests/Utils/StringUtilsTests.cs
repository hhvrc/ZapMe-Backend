using ZapMe.Utils;

namespace ZapMe.Tests.Utils;

public sealed class UtilTests
{
    [Fact]
    public void GenerateRandomString_Length()
    {
        for (int i = 0; i < 512; i++)
        {
            Assert.Equal(i, StringUtils.GenerateUrlSafeRandomString(i).Length);
        }
    }

    [Fact]
    public void GenerateRandomString_InvalidLength()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => StringUtils.GenerateUrlSafeRandomString(-1));
    }
}