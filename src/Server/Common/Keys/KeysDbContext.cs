using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Common.Keys
{
    internal class KeysDbContext : DbContext, IDataProtectionKeyContext
    {
        public KeysDbContext(DbContextOptions<KeysDbContext> options) : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } 
    }
}
