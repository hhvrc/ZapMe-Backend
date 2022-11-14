namespace ZapMe.Tests.Extensions.StringExtensions;

public sealed class TrimAndMinifyWhiteSpacesTests
{
    [Fact]
    public void Empty_ShouldReturn_Empty()
    {
        Assert.Equal(String.Empty, String.Empty.TrimAndMinifyWhiteSpaces());
    }

    [Fact]
    public void One_Word()
    {
        Assert.Equal("abc", "abc".TrimAndMinifyWhiteSpaces());
    }

    [Fact]
    public void Whitespace_Borders()
    {
        Assert.Equal("abc", " abc ".TrimAndMinifyWhiteSpaces());
    }

    [Fact]
    public void Double_Whitespace_Borders()
    {
        Assert.Equal("abc", "  abc  ".TrimAndMinifyWhiteSpaces());
    }

    [Fact]
    public void All_Seperated()
    {
        Assert.Equal("a b c", " a b c ".TrimAndMinifyWhiteSpaces());
    }

    [Fact]
    public void Random_Whitespaces()
    {
        Assert.Equal("a b c", "\r \n \t a  b  c \t \r \n".TrimAndMinifyWhiteSpaces());
    }

    [Fact]
    public void Null_ShouldThrow_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => (null as string)!.TrimAndMinifyWhiteSpaces());
    }
}