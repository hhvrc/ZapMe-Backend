using System.Drawing;
using ZapMe.Views.Generator;

namespace ZapMe.Tests.Views.Generator;

public sealed class HtmlElementTests
{
    [Fact]
    public void TestGeneration()
    {
        HtmlElement body = new HtmlElement(HtmlTagType.Body);
        body.AddAttribute("color", Color.FromArgb(0x69, 0xAB, 0xCD, 0xEF));
        body.AddAttribute("color2", Color.FromArgb(0xFF, 0xFE, 0xDC, 0xBA));
        Assert.NotNull(body);

        // head
        body.AddChild(HtmlTagType.Br);
        body.AddChild(HtmlTagType.Div, ("test", "\"\\am;")).AddChildRaw("  A  \n \r \t  B C ");
        body.AddChild(HtmlTagType.Div).AddChildString("  A  B <p> C </p> ");
        body.AddChild(HtmlTagType.P);
        body.AddChild(HtmlTagType.P).AddChildString("a");
        body.AddChild(HtmlTagType.Style).AddChildRaw(
"""
/* -------------------------------------
    Big comment
------------------------------------- */

tag {
    border: none; /* another * /  **  */
    test: asdasd;
}
""");

        Assert.Equal(
/* lang=html */
"""
<!doctype html><body color="#ABCDEF69" color2="#FEDCBA"><br><div test="\"\\am;">A B C</div><div>  A  B &lt;p&gt; C &lt;/p&gt; </div><p></p><p>a</p><style>tag { border: none; test: asdasd; }</style></body>
""", body.Render());
    }
}
