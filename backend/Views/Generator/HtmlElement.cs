using System.Drawing;
using System.Net;
using System.Text;

namespace ZapMe.Views.Generator;

public sealed partial class HtmlElement
{
    private static string RemoveCommentsManual(ReadOnlySpan<char> input)
    {
        StringBuilder sb = new StringBuilder();

        while (!input.IsEmpty)
        {
            int open = input.IndexOf("/*") + 2;
            switch (open)
            {
                case < 2:
                    sb.Append(input);
                    return sb.ToString();
                case > 2:
                    sb.Append(input[..(open - 2)]);
                    break;
                case 2:
                    break;
            }

            if (open >= input.Length) break;

            input = input[open..];

            int close = input.IndexOf("*/") + 2;
            if (close < 2 || close >= input.Length) break;

            input = input[close..];
        }

        return sb.ToString();
    }

    private string ProcessRawString(string str)
    {
        if (TagType is HtmlTagType.Style)
        {
            str = RemoveCommentsManual(str);
        }

        return str.TrimAndMinifyWhiteSpaces();
    }

    public HtmlElement(HtmlTagType tagType, params (string name, string value)[] attributes)
    {
        TagType = tagType;
        Attributes = attributes.ToDictionary(static x => x.name, static x => x.value);
    }

    public HtmlElement? Parent { get; set; }
    public HtmlTagType TagType { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    public List<object> Children { get; set; } = new List<object>();

    public void AddAttribute(string k, int v) => AddAttribute(k, v);
    public void AddAttribute(string k, Color v) => AddAttribute(k, v.ToHex());
    public void AddAttribute(string k, string v) => Attributes[k] = v;
    public void RemoveAttribute(string k) => Attributes.Remove(k);

    public void AddChild(HtmlElement htmlTag)
    {
        Children.Add(htmlTag);
        htmlTag.Parent = this;
    }
    public void AddChildAt(HtmlElement htmlTag, int index)
    {
        Children.Insert(index, htmlTag);
        htmlTag.Parent = this;
    }
    public void AddChildRaw(string raw) => Children.Add(ProcessRawString(raw));
    public void AddChildRawAt(string raw, int index) => Children.Insert(index, ProcessRawString(raw));
    public void AddChildString(string str) => Children.Add(WebUtility.HtmlEncode(str));
    public void AddChildStringAt(string str, int index) => Children.Insert(index, WebUtility.HtmlEncode(str));
    public void RemoveChild(object obj) => Children.Remove(obj);
    public void RemoveChildAt(int index) => Children.RemoveAt(index);

    public HtmlElement AddChild(HtmlTagType type, params (string name, string value)[] attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        HtmlElement htmlTag = new HtmlElement(type, attributes) { Parent = this };
        Children.Add(htmlTag);
        return htmlTag;
    }


    public string Render()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<!doctype html>");
        Render(sb);
        return sb.ToString();
    }
    private void Render(StringBuilder sb)
    {
        string tag = TagType.ToString().ToLower();

        sb.Append('<');
        sb.Append(tag);

        foreach (KeyValuePair<string, string> attr in Attributes)
        {
            sb.Append(' ');
            sb.Append(attr.Key);
            sb.Append('=');
            sb.Append('"');
            sb.Append(attr.Value.Replace("\\", "\\\\").Replace("\"", "\\\""));
            sb.Append('"');
        }

        if (Children?.Count > 0)
        {
            sb.Append('>');
            if (TagType is HtmlTagType.Br) return;

            foreach (object child in Children)
            {
                if (child is HtmlElement htmlElement)
                {
                    htmlElement.Render(sb);
                }
                else if (child is string str)
                {
                    sb.Append(str);
                }
                else
                {
                    throw new Exception("Invalid type!");
                }
            }

            sb.Append('<');
            sb.Append('/');
            sb.Append(tag);
            sb.Append('>');
        }
        else
        {
            if (TagType is HtmlTagType.Br or HtmlTagType.P)
            {
                sb.Append('>');

                if (TagType is HtmlTagType.P)
                {
                    sb.Append('<');
                    sb.Append('/');
                    sb.Append(tag);
                    sb.Append('>');
                }
            }
            else
            {
                sb.Append('/');
                sb.Append('>');
            }
        }
    }
}
