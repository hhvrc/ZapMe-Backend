using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public static class DatabaseFacadeExtensions
{
    public static async Task<IDbContextTransaction?> BeginTransactionIfNotExistsAsync(this DatabaseFacade db, CancellationToken cancellationToken)
    {
        return db.CurrentTransaction is null ? await db.BeginTransactionAsync(cancellationToken) : null;
    }
}
