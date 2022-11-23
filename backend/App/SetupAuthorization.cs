namespace ZapMe.App;

partial class App
{
    private void AddAuthorization()
    {
        Services.AddAuthorization(opt =>
        {
            // Example:
            // opt.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
        });
    }
}
