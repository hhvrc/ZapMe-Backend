using ZapMe.Utils;

namespace ZapMe.Tests.Utils;

public sealed class HashingUtilsTests
{
    [Theory]
    [InlineData("", "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855")]
    [InlineData("hashme", "02208B9403A87DF9F4ED6B2EE2657EFAA589026B4CCE9ACCC8E8A5BF3D693C86")]
    public void Sha256Hex_String_ReturnsValidHash(string data, string expected)
    {
        // Act
        var result = HashingUtils.Sha256_Hex(data);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", "47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")]
    [InlineData("hashme", "AiCLlAOoffn07Wsu4mV++qWJAmtMzprMyOilvz1pPIY=")]
    public void Sha256Base64_String_ReturnsValidHash(string data, string expected)
    {
        // Act
        var result = HashingUtils.Sha256_Base64(data);

        // Assert
        Assert.Equal(expected, result);
    }
}