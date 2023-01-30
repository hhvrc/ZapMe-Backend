using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;
using ZapMe.Constants;
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

    public async Task<string[]> GetTemplateNamesArrayAsync(CancellationToken cancellationToken = default)
    {
        string[]? list = await _memoryCache.GetOrCreateAsync("EmailTemplateList", async entry => await _emailTemplateStore.GetTemplateNamesArrayAsync(cancellationToken));

        return list ?? Array.Empty<string>();
    }

    public async IAsyncEnumerable<string> GetTemplateNamesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (string item in await GetTemplateNamesArrayAsync(cancellationToken))
        {
            yield return item;
        }
    }

    public Task<string?> GetEmailTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        return _memoryCache.GetOrCreateAsync("EmailTemplate:" + templateName, async entry => await _emailTemplateStore.GetEmailTemplateAsync(templateName, cancellationToken));
    }
}
