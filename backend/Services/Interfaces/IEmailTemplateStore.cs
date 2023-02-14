namespace ZapMe.Services.Interfaces;

public interface IEmailTemplateStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<string[]> GetTemplateNamesArrayAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<string> GetTemplateNamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetEmailTemplateAsync(string templateName, CancellationToken cancellationToken = default);
}
