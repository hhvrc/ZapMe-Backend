using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthorizationIServiceCollectionExtensions
{
    public static void ZMAddAuthorization([NotNull] this IServiceCollection services)
    {
        services.AddAuthorization(opt =>
        {
            // Example:
            // opt.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
        });
    }
}
