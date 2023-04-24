namespace ZapMe.Websocket.Models.ServerPayloads;

public sealed class GlobalMessage
{
    /// <summary>
    /// 
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Message { get; set; }
}
