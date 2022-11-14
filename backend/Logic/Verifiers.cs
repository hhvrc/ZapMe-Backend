using System.Text;

namespace ZapMe.Logic;

public static class Verifiers
{
    // 00300 = Zalgo text (https://en.wikipedia.org/wiki/Zalgo_text)
    // 02000 = Zero-width space (https://en.wikipedia.org/wiki/Zero-width_space)

    // See: https://unicode-explorer.com/b/<commented code>

    /// <summary>
    /// Detects all unwanted characters in a string.
    /// This includes Zalgo text and zero-width spaces.
    /// It detects all Diacritical Marks and Combining Diacritical Marks.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private static bool IsUnwantedRune(int v)
    {
        return v is (>= 0x002B0 and <= 0x0036F) // 002B0, 00300
                 or (>= 0x01AB0 and <= 0x01AFF) // 01AB0
                 or (>= 0x01DC0 and <= 0x01DFF) // 01DC0
                 or (>= 0x02000 and <= 0x0209F) // 02000, 02070
                 or (>= 0x020D0 and <= 0x021FF) // 020D0, 02100, 02150, 02190
                 or (>= 0x02300 and <= 0x023FF) // 02300
                 or (>= 0x02460 and <= 0x024FF) // 02460
                 or (>= 0x025A0 and <= 0x027BF) // 025A0, 02600, 02700
                 or (>= 0x02900 and <= 0x0297F) // 02900
                 or (>= 0x02B00 and <= 0x02BFF) // 02B00
                 or (>= 0x0FE00 and <= 0x0FE0F) // 0FE00
                 or (>= 0x1F000 and <= 0x1F02F) // 1F000
                 or (>= 0x1F0A0);               // 1F0A0, 1F100, 1F200, 1F300, 1F600, 1F650, 1F680, 1F700, 1F780, 1F800, 1F900, 1FA00, 1FA70, 1FB00, 1FF80, 20000, 2A700, 2B740, 2B820, 2CEB0, 2F800, 2FF80, 30000, 3FF80, 4FF80, 5FF80, 6FF80, 7FF80, 8FF80, 9FF80, AFF80, BFF80, CFF80, DFF80, E0000, E0100, EFF80, FFF80, 10FF80
    }

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

        // String was only whitespace
        if (str.Length != len) return true;

        // Check if string contains any unwanted characters
        foreach (Rune c in str.EnumerateRunes())
        {
            if (IsUnwantedRune(c.Value))
            {
                return true;
            }
        }

        return false;
    }
}