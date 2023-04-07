using Common.Mongo;

namespace Identity.Entity
{
    public class User : IEntity<string>
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public List<UserClaim> Claims { get; set; } = new();
        public string Id { get; set; } = "";
    }

    public class UserClaim 
    {
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
