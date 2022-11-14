namespace ZapMe.Tests.Extensions.StringExtensions;

public sealed class ObscureTests
{
    [Fact]
    public void Empty_ShouldReturn_Empty()
    {
        Assert.Equal(String.Empty, String.Empty.Obscure());
    }

    [Fact]
    public void Four_Letters_ShouldReturn_All_Censored()
    {
        Assert.Equal("****", "test".Obscure());
    }

    [Fact]
    public void Five_Letters()
    {
        Assert.Equal("p***t", "point".Obscure());
    }

    [Fact]
    public void Seven_Letters()
    {
        Assert.Equal("o******d", "obscured".Obscure());
    }

    [Fact]
    public void Null_ShouldThrow_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => (null as string)!.Obscure());
    }
}