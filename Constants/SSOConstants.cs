namespace ZapMe.Constants;

public static class SSOConstants
{
    public static readonly TimeSpan StateLifetime = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan RegistrationTicketLifetime = TimeSpan.FromMinutes(15);
}
