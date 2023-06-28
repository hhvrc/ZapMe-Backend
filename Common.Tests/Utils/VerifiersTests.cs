using System.Text.Json;
using ZapMe.Utils;

namespace ZapMe.Tests.Utils;

public sealed class VerifiersTests
{
    private delegate void HandleLineFunc(ReadOnlySpan<char> line);
    private static void GetAllLines(ReadOnlySpan<char> text, HandleLineFunc handleLine)
    {
        while (!text.IsEmpty)
        {
            int textStart = text.IndexOfAnyExcept('\r', '\n');
            if (textStart < 0) return;
            else if (textStart > 0) text = text[textStart..];

            int textEnd = text.IndexOfAny('\r', '\n');
            if (textEnd < 0)
            {
                handleLine(text);
                return;
            }

            handleLine(text[..textEnd]);
            text = text[textEnd..];
        }
    }

    private static IEnumerable<(string fileName, string text)> ReadAllFiles(string folderPath, string searchPattern)
    {
        string[] fileNames = Directory.GetFiles(folderPath, searchPattern);
        Assert.NotNull(fileNames);

        foreach (string fileName in fileNames)
        {
            string text = File.ReadAllText(fileName);
            Assert.NotNull(text);

            yield return (fileName, text);
        }
    }

    private delegate bool TestLineFunc(ReadOnlySpan<char> line);
    private static void Assert_Lines_In_Files(string folderPath, string searchPattern, TestLineFunc testLine)
    {
        foreach ((string fileName, string text) in ReadAllFiles(folderPath, searchPattern))
        {
            GetAllLines(text, line => Assert.True(testLine(line), $"Line \"{line}\" in file {fileName} should pass, but does not."));
        }
    }

    [Fact]
    public void IsBadUiString_WhiteListed_Charsets() => Assert_Lines_In_Files("DataSet/Text/WhiteList/Charsets/", "*.txt", line => !Verifiers.IsBadUiString(line));

    [Fact]
    public void IsBadUiString_WhiteListed_Texts()
    {
        foreach ((string fileName, string text) in ReadAllFiles("DataSet/Text/WhiteList/Text/", "*.txt"))
        {
            Assert.False(Verifiers.IsBadUiString(text), $"File {fileName} should pass, but does not.");
        }
    }

    [Fact]
    public void IsBadUiString_BlackListed_Charsets() => Assert_Lines_In_Files("DataSet/Text/BlackList/Charsets/", "*.txt", Verifiers.IsBadUiString);

    [Fact]
    public void IsBadUiString_BlackListed_Texts()
    {
        foreach ((string fileName, string text) in ReadAllFiles("DataSet/Text/BlackList/Text/", "*.txt"))
        {
            Assert.True(Verifiers.IsBadUiString(text), $"File {fileName} should fail, but does not.");
        }
    }

    [Fact]
    public void IsBadUiString_Bogus_Lorem()
    {
        string localesJson = File.ReadAllText("DataSet/Text/locales.json");
        Assert.False(String.IsNullOrEmpty(localesJson));

        string[]? localesArray = JsonSerializer.Deserialize<string[]>(localesJson);
        Assert.NotNull(localesArray);

        foreach (string locale in localesArray)
        {
            ReadOnlySpan<char> lorem = new Bogus.DataSets.Lorem(locale).Sentence(8192).AsSpan().Trim();

            Assert.False(Verifiers.IsBadUiString(lorem), $"Failed on locale[{locale}]: {lorem}");
        }
    }

    [Fact]
    public void IsBadUiString_NullAndEmpty()
    {
        Assert.True(Verifiers.IsBadUiString(String.Empty));
        Assert.True(Verifiers.IsBadUiString(null!));
    }
}