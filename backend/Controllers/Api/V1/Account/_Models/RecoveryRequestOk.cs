using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Account.Models;

public readonly struct RecoveryRequestOk
{
    /// <summary>
    /// 
    /// </summary>
    public required string Message { get; init; }
}
