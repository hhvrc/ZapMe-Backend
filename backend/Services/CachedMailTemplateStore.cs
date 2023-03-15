using Microsoft.Extensions.Caching.Memory;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class CachedMailTemplateStore : IMailTemplateStore
{
    private readonly MailTemplateStore _emailTemplateStore;
    private readonly IMemoryCache _memoryCache;

    public CachedMailTemplateStore(MailTemplateStore emailTemplateStore, IMemoryCache memoryCache)
    {
        _emailTemplateStore = emailTemplateStore;
        _memoryCache = memoryCache;
    }

    public async Task<string[]> GetTemplateNamesAsync(CancellationToken cancellationToken = default)
    {
        string[]? list = await _memoryCache.GetOrCreateAsync("EmailTemplateList", async entry => await _emailTemplateStore.GetTemplateNamesAsync(cancellationToken));

        return list ?? Array.Empty<string>();
    }

    public Task<string?> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        return _memoryCache.GetOrCreateAsync("EmailTemplate:" + templateName, async entry => await _emailTemplateStore.GetTemplateAsync(templateName, cancellationToken));
    }
}
