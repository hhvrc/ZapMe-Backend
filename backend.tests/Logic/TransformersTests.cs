using ZapMe.Logic;

namespace ZapMe.Tests.Logic;

public sealed class TransformersTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("a", "*")]
    [InlineData("ab", "**")]
    [InlineData("abc", "***")]
    [InlineData("abc.", "***.")]
    [InlineData("a.com", "*.com")]
    [InlineData("ab.com", "**.com")]
    [InlineData("abc.com", "***.com")]
    [InlineData("abcd.com", "****.com")]
    [InlineData("abcde.com", "a***e.com")]
    [InlineData("abcdef.com", "a****f.com")]
    public void ObscureDomain_EmptyOrValid_ReturnsObscured(string input, string expected)
    {
        // Act
        var result = Transformers.ObscureDomain(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData(".")]
    [InlineData("....")]
    [InlineData("..com")]
    [InlineData(".com")]
    [InlineData("example..")]
    public void ObscureDomain_InvalidDomain_ThrowsFormatException(string value)
    {
        Assert.Throws<FormatException>(() => Transformers.ObscureDomain(value));
    }

    [Fact]
    public void ObscureDomain_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Transformers.ObscureDomain(null!));
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData("a@b", "*@*")]
    [InlineData("a.b@c.d", "***@*.d")]
    [InlineData("test@test", "****@****")]
    [InlineData("bread@bread", "b***d@b***d")]
    [InlineData("user.name@example", "u*******e@e*****e")]
    [InlineData("user.name@example.com", "u*******e@e*****e.com")]
    public void ObscureEmail_EmptyOrValid_ReturnsObscured(string input, string expected)
    {
        // Act
        var result = Transformers.ObscureEmail(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("@@example")]
    [InlineData("@example")]
    [InlineData("user@@")]
    [InlineData("user@")]
    [InlineData("example")]
    public void ObscureEmail_InvalidEmail_ThrowsFormatException(string value)
    {
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail(value));
    }

    [Fact]
    public void ObscureEmail_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Transformers.ObscureEmail(null!));
    }

    [Fact]
    public void AnonymizeEmailUser_Normal()
    {
        Assert.Equal("user@example.com", Transformers.AnonymizeEmailUser("firstname.secondname@example.com"));
        Assert.Equal(String.Empty, Transformers.AnonymizeEmailUser(String.Empty));
    }

    [Fact]
    public void AnonymizeEmailUser_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => Transformers.AnonymizeEmailUser(null!));
    }
}