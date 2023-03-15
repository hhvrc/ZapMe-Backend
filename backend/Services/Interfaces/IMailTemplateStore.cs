namespace ZapMe.Services.Interfaces;

public interface IMailTemplateStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<string[]> GetTemplateNamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
}
