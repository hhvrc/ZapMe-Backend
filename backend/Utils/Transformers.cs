using System.Net.Mail;

namespace ZapMe.Utils;

public static class Transformers
{
    public static string ObscureDomain(string domain)
    {
        ArgumentNullException.ThrowIfNull(domain, nameof(domain));
        if (domain.Length == 0) return String.Empty;

        if (Uri.CheckHostName(domain) == UriHostNameType.Unknown) throw new FormatException("Invalid hostname!");

        return domain.ObscureExceptLast('.');
    }

    public static string ObscureEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        if (email.Length == 0) return String.Empty;

        MailAddress parsed = new MailAddress(email);

        return parsed.User.Obscure() + '@' + ObscureDomain(parsed.Host);
    }

    public static string AnonymizeEmailUser(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        if (email.Length == 0) return String.Empty;

        MailAddress parsed = new MailAddress(email);

        return "user@" + parsed.Host;
    }
}
