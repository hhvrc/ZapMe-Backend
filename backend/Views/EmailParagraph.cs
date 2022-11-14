using ZapMe.Views.Generator;

namespace ZapMe.Views;

public static class EmailParagraph
{
    public static HtmlElement Build(string? text = null)
    {
        HtmlElement p = new HtmlElement(HtmlTagType.P);

        if (text != null)
        {
            p.AddChildString(text);
        }

        return p;
    }
}
