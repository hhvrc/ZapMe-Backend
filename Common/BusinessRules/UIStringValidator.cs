using System.Text;

namespace ZapMe.BusinessRules;

public static class UIStringValidator
{

    /// <summary>
    /// Checks if string contains symbols that contains unwanted input from a user like <see href="https://lingojam.com/ZalgoText">Cursed</see>/<see href="https://unicode-table.com/en/200B/">Zero Width Space</see>/Emojis
    /// </summary>
    /// <param name="str">String to check for obnoxious characters</param>
    /// <returns>True if string is safe</returns>
    public static bool IsBadUiString(ReadOnlySpan<char> str)
    {
        int len = str.Length;

        // String is empty
        if (len == 0) return true;

        str = str.Trim();

        // String had whitespace on one or both ends
        if (str.Length != len) return true;

        // Check if string contains any unwanted characters
        foreach (Rune r in str.EnumerateRunes())
        {
            if (CharsetMatchers.IsUnwantedUserInterfaceRune(r))
            {
                return true;
            }
        }

        return false;
    }
}