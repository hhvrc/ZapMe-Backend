namespace ZapMe.App;

partial class App
{
    private void AddLogging()
    {
        Builder.Logging
            .AddSimpleConsole(cnf =>
            {
                cnf.IncludeScopes = true;
                cnf.SingleLine = false;
                cnf.UseUtcTimestamp = true;
                cnf.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
            });
    }
}
