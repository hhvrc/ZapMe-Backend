using System.Runtime.InteropServices;

namespace ZapMe.Helpers;

/// <summary>
/// The QuickStringReplacer class allows for replacing multiple occurrences of a substring with a new value in a given source string. 
/// It is a simple and fast alternative to String.Replace(), Regex.Replace(), and StringBuilder.Replace().
/// </summary>
public sealed class QuickStringReplacer
{
    private readonly record struct Replacement(int MatchStart, int MatchEnd, string New);

    private readonly string _source;
    private readonly List<Replacement> _replacements = new(8);
    private int _diff;

    /// <summary>
    /// Constructor that takes the source string as a parameter, this is the string that will be modified.
    /// </summary>
    /// <param name="source">the source string to be modified</param>
    public QuickStringReplacer(string source)
    {
        _source = source;
    }

    /// <summary>
    /// Adds a new replacement operation to the list of replacements to be performed.
    /// </summary>
    /// <param name="oldValue">The substring to be replaced</param>
    /// <param name="newValue">The new value to replace the substring with</param>
    /// <returns>This QuickStringReplacer instance with the Replace method applied, use this for method chaining</returns>
    public QuickStringReplacer Replace(ReadOnlySpan<char> oldValue, string newValue)
    {
        ArgumentNullException.ThrowIfNull(newValue);

        // Skip empty match strings
        int oldLength = oldValue.Length;
        if (oldLength <= 0)
        {
            return this;
        }

        ReadOnlySpan<char> source = _source.AsSpan();
        int diff = newValue.Length - oldLength;
        int abs = 0, rel;

        // Find all matches
        while ((rel = source[abs..].IndexOf(oldValue)) != -1)
        {
            int pos = abs + rel;

            // Add replacement action
            _replacements.Add(new(pos, abs = pos + oldLength, newValue));

            // Set offset to end of match
            _diff += diff;
        }

        // Return this instance for method chaining
        return this;
    }

    private static int Compare(Replacement left, Replacement right) => left.MatchStart - right.MatchStart;
    private static readonly Comparison<Replacement> _Comparison = new(Compare);

    /// <summary>
    /// Returns the modified source string with all the replacements applied.
    /// </summary>
    /// <returns>The modified string with all the replacements applied.</returns>
    public override string ToString()
    {
        int newLength = _source.Length + _diff;

        // Optimized method for creating string without having to allocate a new char[] array
        return String.Create(newLength, this, static (span, state) =>
        {
            ReadOnlySpan<char> source = state._source.AsSpan();
            Span<Replacement> replacements = CollectionsMarshal.AsSpan(state._replacements);

            // Sort replacements by position in source string
            replacements.Sort(_Comparison);

            int offset = 0, prev = 0;

            // Apply replacements
            foreach (Replacement r in replacements)
            {
                // If there is a gap between the previous match and this one, copy the gap
                int chunk = r.MatchStart - prev;
                if (chunk > 0)
                {
                    source.Slice(prev, chunk).CopyTo(span[offset..]);
                    offset += chunk;
                }

                // Copy replacement string if it is not empty
                ReadOnlySpan<char> rNew = r.New.AsSpan();
                if (!rNew.IsEmpty)
                {
                    rNew.CopyTo(span[offset..]);
                    offset += rNew.Length;
                }

                // Set offset to end of match
                prev = r.MatchEnd;
            }

            // Copy remaining characters
            int remainder = source.Length - prev;
            if (remainder > 0)
            {
                source[prev..].CopyTo(span[offset..]);
            }
        });
    }
}