using ZapMe.Utils;

namespace ZapMe.Tests.Utils;

public sealed class UtilTests
{
    [Fact]
    public void GenerateRandomString_Length()
    {
        for (int i = 0; i < 1000; i++)
        {
            Assert.Equal(32, StringUtils.GenerateUrlSafeRandomString(32).Length);
        }
    }
}