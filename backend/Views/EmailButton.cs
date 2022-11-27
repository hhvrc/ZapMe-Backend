using ZapMe.Views.Generator;

namespace ZapMe.Views;

public static class EmailButton
{
    public static HtmlElement Build(string text, string url)
    {
        HtmlElement table = new HtmlElement(HtmlTagType.Table, ("class", "btn btn-primary"), ("role", "presentation"), ("border", "0"), ("cellpadding", "0"), ("cellspacing", "0"));

        table
            .AddChild(HtmlTagType.Tbody)
            .AddChild(HtmlTagType.Tr)
            .AddChild(HtmlTagType.Td, ("align", "center"))
            .AddChild(HtmlTagType.Table, ("role", "presentation"), ("border", "0"), ("cellpadding", "0"), ("cellspacing", "0"))
            .AddChild(HtmlTagType.Tbody)
            .AddChild(HtmlTagType.Tr)
            .AddChild(HtmlTagType.Td)
            .AddChild(HtmlTagType.A, ("href", url), ("target", "_blank"))
            .AddChildString(text);

        return table;
    }
}
