using Marten.Schema;

namespace ZapMe.Database.Documents;

[DocumentAlias("user_control_grant")]
public sealed class ControlGrantDetails
{
    /// <summary>
    /// The ID of the <see cref="Models.ControlGrantEntity"/> that this document represents.
    /// </summary>
    public Guid Id { get; init; }



}