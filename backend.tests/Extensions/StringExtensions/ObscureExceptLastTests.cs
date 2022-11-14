namespace ZapMe.Tests.Extensions.StringExtensions;

public sealed class ObscureExceptLastTests
{
    [Fact]
    public void Empty_ShouldReturn_Empty()
    {
        Assert.Equal(String.Empty, String.Empty.ObscureExceptLast(' '));
    }

    [Fact]
    public void Two_Words_ShouldReturn_First_Word_Censored()
    {
        Assert.Equal("s****t string", "secret string".ObscureExceptLast(' '));
    }

    [Fact]
    public void Null_ShouldThrow_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => (null as string)!.ObscureExceptLast(' '));
    }
}