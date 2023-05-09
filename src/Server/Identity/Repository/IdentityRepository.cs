using Identity.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Repository
{
    public class IdentityRepository : IdentityDbContext<ApplicationUser>
    {
        public IdentityRepository(DbContextOptions<IdentityRepository> options) : base(options)
        {
        }
    }
}
