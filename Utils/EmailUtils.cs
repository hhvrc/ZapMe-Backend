using System.Runtime.CompilerServices;

namespace ZapMe.Utils;

public static class EmailUtils
{
    public readonly struct ParsedResult
    {
        public static ParsedResult Invalid => new ParsedResult();

        private readonly string _str;
        private readonly int _addrStartIndex;
        private readonly int _atIndex;
        private readonly int _plusIndex;
        private readonly int _addrStopIndex;


        public ParsedResult()
        {
            _str = String.Empty;
            _addrStartIndex = -1;
            _atIndex = -1;
            _plusIndex = -1;
            _addrStopIndex = -1;
        }
        public ParsedResult(string str, int addrStartIndex, int atIndex, int plIndex, int addrStopIndex)
        {
            _str = str;
            _addrStartIndex = addrStartIndex;
            _atIndex = atIndex;
            _plusIndex = plIndex;
            _addrStopIndex = addrStopIndex;
        }

        public bool Success => _atIndex != -1;
        public bool HasDisplayName => _addrStartIndex > 0;
        public bool HasAlias => _plusIndex != -1;
        public ReadOnlySpan<char> EmailAddress => _str;
        public ReadOnlySpan<char> DisplayName => EmailAddress[..(_addrStartIndex - 2)];
        public ReadOnlySpan<char> User => EmailAddress[_addrStartIndex.._atIndex];
        public ReadOnlySpan<char> UserAlias => EmailAddress[_addrStartIndex.._plusIndex];
        public ReadOnlySpan<char> UserAddr => EmailAddress[_addrStartIndex.._plusIndex];
        public ReadOnlySpan<char> Host => EmailAddress[(_atIndex + 1).._addrStopIndex];

        public static implicit operator bool(ParsedResult parsedEmail) => parsedEmail.Success;

        public override string ToString() => _str;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidUser(ReadOnlySpan<char> str)
    {
        if (str.IsEmpty && str.Length > 64)
            return false;

        int lastDot = -1;

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            // Optimistic checking leads to better performance
            if (!CharsetMatchers.IsValidRfcEmailAtomTextRune(c))
            {
                // Two periods in a row is not allowed
                if (c != '.' || lastDot == i - 1)
                {
                    return false;
                }

                lastDot = i;
            }
        }

        return lastDot != str.Length - 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidHost(ReadOnlySpan<char> str)
    {
        if (str.Length < 3)
            return false;

        int dotPos = 0;
        bool wasAlpha = false;

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            switch (c)
            {
                case '.':
                    int len = i - dotPos;
                    if (!wasAlpha || len is 0 or >= 64)
                        return false;
                    dotPos = i;
                    wasAlpha = false;
                    continue;
                case '-':
                    if (!wasAlpha)
                        return false;
                    wasAlpha = false;
                    continue;
                default:
                    if (!CharsetMatchers.IsAlphaNumericLower(c))
                        return false;
                    wasAlpha = true;
                    continue;
            }
        }

        return dotPos != 0 && wasAlpha;
    }

    /// <summary>
    /// This method is really restrictive because I don't want to implement the full <see href="https://www.rfc-editor.org/rfc/rfc2822">RFC</see> for email addresses
    /// </summary>
    /// <param name="str">Trimmed email address to test</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsedResult Parse(string str)
    {
        if (str is null)
            return ParsedResult.Invalid;

        ReadOnlySpan<char> email = str.AsSpan();

        // "a@b.c" (5 chars)
        if (email.Length < 5)
            return ParsedResult.Invalid;

        // @ cannot be the first character
        int atPos = email.LastIndexOf('@');
        if (atPos < 1)
            return ParsedResult.Invalid;

        int addrStart = email.IndexOf('<') + 1;
        int addrStop = email.Length;
        if (addrStart > 0)
        {
            if (atPos < addrStart || email[^1] != '>')
                return ParsedResult.Invalid;

            ReadOnlySpan<char> displayName = email[..(addrStart - 1)].Trim();

            if (displayName.Length == 0 || displayName.Contains('>'))
                return ParsedResult.Invalid;

            addrStop -= 1;
        }

        ReadOnlySpan<char> userPart = email[addrStart..atPos];
        if (!IsValidUser(userPart))
            return ParsedResult.Invalid;

        if (!IsValidHost(email[(atPos + 1)..addrStop]))
            return ParsedResult.Invalid;

        int plusPos = addrStart + userPart.IndexOf('+');

        return new ParsedResult(str, addrStart, atPos, plusPos, addrStop);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsValid(string str) => Parse(str).Success;
}
