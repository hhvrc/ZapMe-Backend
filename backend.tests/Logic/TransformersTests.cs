using ZapMe.Logic;

namespace ZapMe.Tests.Logic;

public sealed class TransformersTests
{
    [Fact]
    public void ObscureDomain_Normal()
    {
        Assert.Equal("***", Transformers.ObscureDomain("wtf"));
        Assert.Equal("***.", Transformers.ObscureDomain("wtf."));
        Assert.Equal("*.com", Transformers.ObscureDomain("a.com"));
        Assert.Equal("**.com", Transformers.ObscureDomain("ez.com"));
        Assert.Equal("***.com", Transformers.ObscureDomain("lol.com"));
        Assert.Equal("****.com", Transformers.ObscureDomain("test.com"));
        Assert.Equal("b***d.com", Transformers.ObscureDomain("bread.com"));
        Assert.Equal("e*****e.com", Transformers.ObscureDomain("example.com"));
        Assert.Equal(String.Empty, Transformers.ObscureDomain(String.Empty));
    }

    [Fact]
    public void ObscureDomain_ThrowsException()
    {
        Assert.Throws<FormatException>(() => Transformers.ObscureDomain("example.."));
        Assert.Throws<FormatException>(() => Transformers.ObscureDomain(".com"));
        Assert.Throws<FormatException>(() => Transformers.ObscureDomain("..com"));
        Assert.Throws<FormatException>(() => Transformers.ObscureDomain("...."));
        Assert.Throws<FormatException>(() => Transformers.ObscureDomain("."));
        Assert.Throws<ArgumentNullException>(() => Transformers.ObscureDomain(null!));
    }

    [Fact]
    public void ObscureEmail_Normal()
    {
        Assert.Equal("*@*", Transformers.ObscureEmail("a@b"));
        Assert.Equal("***@*.d", Transformers.ObscureEmail("a.b@c.d"));
        Assert.Equal("****@****", Transformers.ObscureEmail("test@test"));
        Assert.Equal("b***d@b***d", Transformers.ObscureEmail("bread@bread"));
        Assert.Equal("u*******e@e*****e", Transformers.ObscureEmail("user.name@example"));
        Assert.Equal("u*******e@e*****e.com", Transformers.ObscureEmail("user.name@example.com"));
        Assert.Equal(String.Empty, Transformers.ObscureEmail(String.Empty));
    }

    [Fact]
    public void ObscureEmail_ThrowsException()
    {
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail("example"));
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail("user@"));
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail("user@@"));
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail("@example"));
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail("@@example"));
        Assert.Throws<FormatException>(() => Transformers.ObscureEmail("@"));
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