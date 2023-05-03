using Identity.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Identity.Repository
{
    public class UserRepository
    {
        private static string UserHash(string username) {
            var hash = Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(username)));
            hash = hash.Replace(@"\", "");
            hash = hash.Replace(@"/", "");
            return hash;
        }

        public async Task<User> Get(string email)
        {
            Console.WriteLine(email);
            var hash = UserHash(email.ToString());
            Console.WriteLine(hash);
            if (!File.Exists(hash))
            {
                throw new ApplicationException("User not found");
            }

            await using var reader = File.OpenRead(hash);
            var serializedUser = await JsonSerializer.DeserializeAsync<User>(reader);
            if (serializedUser == null)
            {
                throw new ApplicationException("User record failed to deserialize");
            }
            return serializedUser;
        }

        public async Task Create(User user)
        {
            Console.WriteLine(user.Email);
            var hash = UserHash(user.Email);
            Console.WriteLine(hash);
            if (File.Exists(hash))
            {
                throw new ApplicationException("User already exists");
            }

            await using var writer = File.OpenWrite(hash);
            await JsonSerializer.SerializeAsync(writer, user);
        }
    }
}
