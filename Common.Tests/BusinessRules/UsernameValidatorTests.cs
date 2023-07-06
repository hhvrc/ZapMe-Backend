using ZapMe.BusinessRules;

namespace ZapMe.Common.Tests.BusinessRules;

public sealed class UsernameValidatorTests
{
    [Fact]
    public void Validate_ValidUsername_ReturnsSuccess()
    {
        // Arrange
        string username = "username";

        // Act
        var result = UsernameValidator.Validate(username);

        // Assert
        Assert.True(result.Ok);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("ab")]
    public void Validate_TooShort_ReturnsFailure(string username)
    {
        // Act
        var result = UsernameValidator.Validate(username);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Username is too short.", result.ErrorMessage);
    }

    [Fact]
    public void Validate_TooLong_ReturnsFailure()
    {
        // Arrange
        string username = new string('a', 33);

        // Act
        var result = UsernameValidator.Validate(username);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Username is too long.", result.ErrorMessage);
    }

    [Theory]
    [InlineData(" username")]
    [InlineData("username ")]
    [InlineData(" username ")]
    [InlineData("username\t")]
    [InlineData("\tusername")]
    [InlineData("\tusername\t")]
    [InlineData("username\r")]
    [InlineData("\rusername")]
    [InlineData("\rusername\r")]
    [InlineData("username\n")]
    [InlineData("\nusername")]
    [InlineData("\nusername\n")]
    [InlineData("username\v")]
    [InlineData("\vusername")]
    [InlineData("\vusername\v")]
    [InlineData("username\f")]
    [InlineData("\fusername")]
    [InlineData("\fusername\f")]
    [InlineData("username\u0085")]
    [InlineData("\u0085username")]
    [InlineData("\u0085username\u0085")]
    [InlineData("username\u00A0")]
    [InlineData("\u00A0username")]
    [InlineData("\u00A0username\u00A0")]
    [InlineData("username\u1680")]
    [InlineData("\u1680username")]
    [InlineData("\u1680username\u1680")]
    [InlineData("username\u2000")]
    [InlineData("\u2000username")]
    [InlineData("\u2000username\u2000")]
    [InlineData("username\u2001")]
    [InlineData("\u2001username")]
    [InlineData("\u2001username\u2001")]
    public void Validate_Whitespace_ReturnsFailure(string username)
    {
        // Act
        var result = UsernameValidator.Validate(username);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Username cannot start or end with whitespace.", result.ErrorMessage);
    }

    [Fact]
    public void Validate_BadUiString_ReturnsFailure()
    {
        // Arrange
        string username = "username😎"; // We don't need to test more than this, the tests for all the other bad stuff is in another test class.

        // Act
        var result = UsernameValidator.Validate(username);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Username must not contain obnoxious characters.", result.ErrorMessage);
    }

    [Theory]
    [InlineData("example@email.com")]
    [InlineData("Example <example@email.com>")]
    public void Validate_Email_ReturnsFailure(string username)
    {
        // Act
        var result = UsernameValidator.Validate(username);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Username must not be an email address.", result.ErrorMessage);
    }
}