namespace ZapMe.Authentication;

public class SignInProperties
{
    public SignInProperties(bool rememberMe, string sessionName, string sessionIp, string sessionCountry, string sessionUserAgent)
    {
        RememberMe = rememberMe;
        SessionName = sessionName;
        SessionIp = sessionIp;
        SessionCountry = sessionCountry;
        SessionUserAgent = sessionUserAgent;
    }

    public bool RememberMe { get; set; }
    public string SessionName { get; set; }
    public string SessionIp { get; set; }
    public string SessionCountry { get; set; }
    public string SessionUserAgent { get; set; }
}
