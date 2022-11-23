namespace ZapMe.App;

partial class App
{
    private void UseExceptionHandler()
    {
        if (IsDevelopment)
        {
            Application.UseDeveloperExceptionPage();
        }
        else
        {
            Application.UseExceptionHandler("/error");
        }
    }
    private void UseHSTS()
    {
        if (!IsDevelopment)
        {
            Application.UseHsts();
        }
    }
    private void UseHttpsRedirection()
    {
    }
    private void UseStaticFiles()
    {
        Application.UseDefaultFiles();
        Application.UseStaticFiles();
    }
    private void UseRouting()
    {
        Application.UseRouting();
    }
    private void UseCORS()
    {
        if (IsDevelopment)
        {
            Application.UseCors("dev");
        }
    }
    private void UseAuthentication()
    {
        Application.UseAuthentication();
    }
    private void UseAuthorization()
    {
        Application.UseAuthorization();
    }
    private void UseBuiltInMiddlewares()
    {
        Application.UseRateLimiter(); // As early as possible
    }
    private void UseCustomMiddlewares()
    {
        // App!.UseHealthChecks("/api/v1/health/"); // unusual // TODO: explore this
        UseSwagger();
        Application.UseMiddleware<Middlewares.ActivityTracker>();
    }
    private void UseEndpoint()
    {
        Application.MapControllers();
    }
}
