using ZapMe.Utils;

namespace ZapMe.Tests.Utils;

public sealed class UtilTests
{
    [Fact]
    public void GenerateRandomString_Length()
    {
        Assert.Equal(32, StringUtils.GenerateUrlSafeRandomString(32).Length);
    }
}