using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class EmailTemplateStore : IEmailTemplateStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<EmailTemplateStore> _logger;

    public EmailTemplateStore(ZapMeContext dbContext, ILogger<EmailTemplateStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string[]> GetTemplateNamesArrayAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.EmailTemplates.Select(static t => t.Name).ToArrayAsync(cancellationToken);
    }

    public IAsyncEnumerable<string> GetTemplateNamesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.EmailTemplates.Select(static t => t.Name).ToAsyncEnumerable();
    }

    public Task<string?> GetEmailTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        return _dbContext.EmailTemplates.Where(t => t.Name == templateName).Select(static t => t.Body).FirstOrDefaultAsync(cancellationToken);
    }
}
