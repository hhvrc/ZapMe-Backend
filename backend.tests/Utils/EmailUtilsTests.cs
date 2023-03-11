using ZapMe.Utils;

namespace ZapMe.Tests.Utils;

public sealed class EmailUtilsTests
{
    [Theory]
    [InlineData("email")]
    [InlineData("firstname.lastname")]
    [InlineData("first+name+lastname")]
    [InlineData("\"email\"")]
    [InlineData("1234567890")]
    [InlineData("_________")]
    [InlineData("firstname-lastname")]
    [InlineData("\"much.more\\ unusual\"")]
    [InlineData("\"very.unusual.@.unusual.com\"")]
    [InlineData("\"very.(),:;<>[].VERY.\\\"very@\\\\\\ very\\\".unusual\"")]
    public void IsValidUser_ValidUser_ReturnsSuccess(string user)
    {
        // Act
        var result = EmailUtils.IsValidUser(user);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("email@example")]
    [InlineData(".email")]
    [InlineData("email.")]
    [InlineData("email..")]
    [InlineData("あいうえお")]
    [InlineData("Abc..123")]
    [InlineData("\"(),:;<>[\\]")]
    [InlineData("just\"not\"right")]
    public void IsValidUser_InvalidUser_ReturnsFailure(string user)
    {
        // Act
        var result = EmailUtils.IsValidUser(user);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("example.com")]
    [InlineData("example-host.com")]
    public void IsValidHost_ValidHost_ReturnsSuccess(string host)
    {
        // Act
        var result = EmailUtils.IsValidHost(host);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(".com")]
    [InlineData("com.")]
    [InlineData("example..com")]
    public void IsValidHost_InvalidHost_ReturnsFailure(string host)
    {
        // Act
        var result = EmailUtils.IsValidHost(host);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Parse_ValidEmail_ReturnsSuccess()
    {
        // Arrange
        const string email = "Display name <alias+user.example@domain-name.com>";

        // Act
        var result = EmailUtils.Parse(email);

        // Assert
        Assert.True(result.Success, "Parse failed");
        Assert.Equal(email, result.ToString());
        Assert.True(result.HasAlias, "Parsed email does not contain input alias");
        Assert.True(result.HasDisplayName, "Parsed email does not contain input display name");
        Assert.Equal("Display name", result.DisplayName.ToString());
        Assert.Equal("alias", result.UserAlias.ToString());
        Assert.Equal("alias+user.example", result.User.ToString());
        Assert.Equal("domain-name.com", result.Host.ToString());
    }
}
