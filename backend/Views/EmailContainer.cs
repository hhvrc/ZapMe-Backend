using ZapMe.Views.Generator;

namespace ZapMe.Views;

public static class EmailContainer
{
    public static HtmlElement Build(params HtmlElement[] elements)
    {
        HtmlElement tr = new HtmlElement(HtmlTagType.Tr);

        HtmlElement td = tr
            .AddChild(HtmlTagType.Td, ("class", "wrapper"))
            .AddChild(HtmlTagType.Table, ("role", "presentation"), ("border", "0"), ("cellpadding", "0"), ("cellspacing", "0"))
            .AddChild(HtmlTagType.Tr)
            .AddChild(HtmlTagType.Td);

        foreach (HtmlElement element in elements)
        {
            td.AddChild(element);
        }

        return tr;
    }
}
