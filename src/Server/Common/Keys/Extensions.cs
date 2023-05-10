using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Keys
{
    public static class Extensions
    {
        public static IServiceCollection AddKeysStorage(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<KeysDbContext>(c =>
            {
                c.UseSqlServer(connectionString);
            });
            services.AddDataProtection().SetApplicationName("PartyFinder").PersistKeysToDbContext<KeysDbContext>();
            return services;
        }
    }
}
