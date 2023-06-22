using Microsoft.Extensions.Logging;

namespace ZapMe.Database;

public static class DatabaseContextExceptions
{
    public static async Task<T> GetOrAddAsync<T>(this DatabaseContext ctx, Func<DatabaseContext, Task<T>> entitySelector, Func<T> factory, CancellationToken cancellationToken) where T : class
    {
        // Try to get entity from database
        T entity = await entitySelector(ctx);
        if (entity is not null)
        {
            return entity;
        }

        // Try to create entity
        entity = factory();
        ctx.Add(entity);
        await ctx.SaveChangesAsync(cancellationToken);

        // Possible race condition, try to get entity one last time
        return await entitySelector(ctx);
    }
}
