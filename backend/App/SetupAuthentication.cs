using ZapMe.Authentication;

namespace ZapMe.App;

partial class App
{
    private void AddAuthentication()
    {
        Services
            .AddAuthentication(ZapMeAuthenticationDefaults.AuthenticationScheme)
            .AddZapMe(opt =>
            {
                CookieBuilder cookie = opt.Cookie;

                cookie.Name = Constants.LoginCookieName;
                cookie.HttpOnly = true;
                cookie.IsEssential = true;
                cookie.SameSite = SameSiteMode.Strict;
                cookie.SecurePolicy = CookieSecurePolicy.Always;
                cookie.MaxAge = TimeSpan.FromDays(30);

                // CookieAuthenticationOptions
                opt.ExpireTimeSpan = TimeSpan.FromDays(30);
                opt.SlidingExpiration = true;
            })
            .AddGoogle(opt =>
            {
                opt.ClientId = Configuration["Authorization:Google:ClientId"]!;
                opt.ClientSecret = Configuration["Authorization:Google:ClientSecret"]!;
                opt.CallbackPath = Configuration["Authorization:Google:CallbackPath"]!;
                opt.Scope.Add("openid");
                opt.Scope.Add(".../auth/userinfo.email");
            })
            .AddGitHub(opt =>
            {
                opt.ClientId = Configuration["Authorization:GitHub:ClientId"]!;
                opt.ClientSecret = Configuration["Authorization:GitHub:ClientSecret"]!;
                opt.CallbackPath = Configuration["Authorization:GitHub:CallbackPath"]!;
                opt.Scope.Add("user:email");
            })/*
            .AddTwitter(options => {
            })*/;
    }
}
