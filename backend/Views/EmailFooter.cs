using ZapMe.Views.Generator;

namespace ZapMe.Views;

public static class EmailFooter
{
    public static HtmlElement Build(string companyName, string companyAddress, string poweredBy, string poweredByLink, string? unsubscriptionLink = null)
    {
        HtmlElement div = new HtmlElement(HtmlTagType.Div, ("class", "footer"));
        HtmlElement table = div.AddChild(HtmlTagType.Table, ("role", "presentation"), ("border", "0"), ("cellpadding", "0"), ("cellspacing", "0"), ("width", "100%"));

        // table
        HtmlElement td1 = table
            .AddChild(HtmlTagType.Tr)
            .AddChild(HtmlTagType.Td, ("class", "content-block"));
        HtmlElement td2 = table
            .AddChild(HtmlTagType.Tr)
            .AddChild(HtmlTagType.Td, ("class", "content-block powered-by"));

        // td1
        td1.AddChild(HtmlTagType.Span, ("class", "apple-link")).AddChildString($"{companyName}, {companyAddress}");
        if (!String.IsNullOrEmpty(unsubscriptionLink))
        {
            HtmlElement br = td1.AddChild(HtmlTagType.Br);
            br.AddChildString("Don't like these emails?");
            br.AddChild(HtmlTagType.A, ("href", unsubscriptionLink)).AddChildString("Unsubscribe");
            br.AddChildString(".");
        }

        // td2
        td2.AddChildString("Powered by ");
        td2.AddChild(HtmlTagType.A, ("href", poweredByLink)).AddChildString(poweredBy);
        td2.AddChildString(".");

        return div;
    }
}
