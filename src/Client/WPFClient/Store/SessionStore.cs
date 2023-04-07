using WPFClient.Model;

namespace WPFClient.Store
{
    public class SessionStore
    {
        public User User;

        public SessionStore()
        {
            this.User = new User();
        }
    }
}
