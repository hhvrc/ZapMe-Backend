using Microsoft.Extensions.Caching.Memory;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class CachedEmailTemplateStore : IEmailTemplateStore
{
    private readonly EmailTemplateStore _emailTemplateStore;
    private readonly IMemoryCache _memoryCache;

    public CachedEmailTemplateStore(EmailTemplateStore emailTemplateStore, IMemoryCache memoryCache)
    {
        _emailTemplateStore = emailTemplateStore;
        _memoryCache = memoryCache;
    }

    public async Task<OneOf<string[], ErrorDetails>> GetTemplateNamesAsync(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue("EmailTemplateList", out string[]? nameList) && nameList != null)
        {
            return nameList;
        }

        using ICacheEntry entry = _memoryCache.CreateEntry("EmailTemplateList");

        OneOf<string[], ErrorDetails> result = await _emailTemplateStore.GetTemplateNamesAsync(cancellationToken);

        entry.Value = result.IsT0 ? result.AsT0 : null;

        return result;
    }

    public async Task<OneOf<string, ErrorDetails>> GetTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        string key = "EmailTemplate:" + templateName;

        if (_memoryCache.TryGetValue(key, out string? template) && template != null)
        {
            return template;
        }

        using ICacheEntry entry = _memoryCache.CreateEntry(key);

        OneOf<string, ErrorDetails> result = await _emailTemplateStore.GetTemplateAsync(templateName, cancellationToken);

        entry.Value = result.IsT0 ? result.AsT0 : null;

        return result;
    }
}
