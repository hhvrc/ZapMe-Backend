using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingILoggingBuilderExtensions
{
    public static void ZMAddLogging([NotNull] this ILoggingBuilder builder)
    {
        builder.AddSimpleConsole(cnf =>
        {
            cnf.IncludeScopes = true;
            cnf.SingleLine = false;
            cnf.UseUtcTimestamp = true;
            cnf.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
        });
    }
}
