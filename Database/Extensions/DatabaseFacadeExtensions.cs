using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ZapMe.Database;

public static class DatabaseFacadeExtensions
{

    public static async Task<IDbContextTransaction?> BeginTransactionIfNotExistsAsync(this DatabaseFacade db, CancellationToken cancellationToken)
    {
        return db.CurrentTransaction is null ? await db.BeginTransactionAsync(cancellationToken) : null;
    }
}
