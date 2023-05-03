using Common.Mongo;

namespace Identity.Entity
{
    public class User
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Email { get; set; } = "";
        public string DiscordId { get; set; } = "";
        public List<UserClaim> Claims { get; set; } = new();
        public Guid Id { get; set; }
    }

    public class UserClaim 
    {
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
