using Common.Mongo;
using Microsoft.AspNetCore.Identity;

namespace Identity.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string DiscordId { get; set; } = string.Empty;
    }
}
