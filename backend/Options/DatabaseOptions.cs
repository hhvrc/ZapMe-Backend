namespace ZapMe.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string ConnectionString { get; set; }
}
