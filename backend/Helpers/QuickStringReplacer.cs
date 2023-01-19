namespace ZapMe.Helpers;

public sealed class QuickStringReplacer
{
    private readonly struct ReplaceAction
    {
        public int MatchStart { get; init; }
        public int MatchEnd { get; init; }
        public string Replacement { get; init; }
    }

    private readonly string _value;
    private readonly List<ReplaceAction> _replacements;
    private int _sizeDiff;

    public QuickStringReplacer(string value)
    {
        _value = value;
        _replacements = new List<ReplaceAction>();
        _sizeDiff = 0;
    }

    public QuickStringReplacer Replace(ReadOnlySpan<char> match, string replacement)
    {
        int matchLength = match.Length;
        if (matchLength > 0)
        {
            ReadOnlySpan<char> value = _value.AsSpan();
            int sizeDiff = replacement.Length - matchLength;

            int relativeIndex;
            int absolutePosition = 0;

            while ((relativeIndex = value.IndexOf(match)) != -1)
            {
                int absoluteStart = absolutePosition + relativeIndex;
                int relativeEnd = relativeIndex + matchLength;

                _replacements.Add(new ReplaceAction
                {
                    MatchStart = absoluteStart,
                    MatchEnd = absoluteStart + matchLength,
                    Replacement = replacement
                });
                _sizeDiff += sizeDiff;

                value = value[relativeEnd..];
                absolutePosition += relativeEnd;
            }
        }

        return this;
    }

    public override string ToString()
    {
        int newStringSize = _value.Length + _sizeDiff;
        _replacements.Sort(static (x, y) => x.MatchStart - y.MatchStart);

        return String.Create(newStringSize, this, static (span, qsr) =>
        {
            int resizedSize = 0;
            int valueEndIndex = 0;
            ReadOnlySpan<char> value = qsr._value;

            foreach (ReplaceAction action in qsr._replacements)
            {
                int originalChunkLength = action.MatchStart - valueEndIndex;
                if (originalChunkLength > 0)
                {
                    value.Slice(valueEndIndex, originalChunkLength).CopyTo(span[resizedSize..]);
                    resizedSize += originalChunkLength;
                }

                ReadOnlySpan<char> replacement = action.Replacement.AsSpan();
                if (!replacement.IsEmpty)
                {
                    replacement.CopyTo(span[resizedSize..]);
                    resizedSize += replacement.Length;
                }

                valueEndIndex = action.MatchEnd;
            }

            int remainingLength = value.Length - valueEndIndex;
            if (remainingLength > 0)
            {
                value.Slice(valueEndIndex, remainingLength).CopyTo(span[resizedSize..]);
            }
        });
    }
}