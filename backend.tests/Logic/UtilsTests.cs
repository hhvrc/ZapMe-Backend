using ZapMe.Logic;

namespace ZapMe.Tests.Logic;

public sealed class UtilTests
{
    [Fact]
    public void GenerateRandomString_Length()
    {
        for (int i = 0; i < 1000; i++)
        {
            Assert.Equal(32, Utils.GenerateRandomString(32).Length);
        }
    }
}