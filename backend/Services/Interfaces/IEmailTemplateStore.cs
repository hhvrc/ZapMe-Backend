using OneOf;
using ZapMe.Controllers.Api.V1.Models;

namespace ZapMe.Services.Interfaces;

public interface IEmailTemplateStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<OneOf<string[], ErrorDetails>> GetTemplateNamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<string, ErrorDetails>> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
}
