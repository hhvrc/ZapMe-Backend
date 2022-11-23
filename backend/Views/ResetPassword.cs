using System.Drawing;
using ZapMe.Views.Generator;

namespace ZapMe.Views;

public static class ResetPassword
{
    public static string Build(string username, string resetPasswordUrl, Color? backgroundColor = null)
    {
        HtmlElement container = EmailContainer.Build(
                            EmailParagraph.Build($"Hey {username},"),
                            EmailParagraph.Build(),
                            EmailParagraph.Build("You requested a password recovery for your account."),
                            EmailParagraph.Build("If you did not request this, please ignore this email."),
                            EmailParagraph.Build("If you did request this, please click the button below to reset your password."),
                            EmailParagraph.Build(),
                            EmailButton.Build("Reset password", resetPasswordUrl)
                        );

        HtmlElement footer = EmailFooter.Build(Constants.AppCreator, Constants.MadeInText, Constants.AppName, Constants.MainPageUrl);

        HtmlElement document = EmailDocument.Build("Password reset request", "Password reset request", new HtmlElement[] { container }, footer, backgroundColor);

        return PreMailer.Net.PreMailer.MoveCssInline(document.Render(), preserveMediaQueries: true).Html;
    }
}