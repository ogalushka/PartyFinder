using Common.Mongo;
using Identity.Entity;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace UserService.Repository
{
    //TODO better interface or remove it completly, move users to DB
    public class UserRepository : IRepository<string, User>
    {
        private static string UserHash(string username) => Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(username)));

        public async Task<User> Get(string username)
        {
            var hash = UserHash(username);
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
            var hash = UserHash(user.Username);
            await using var writer = File.OpenWrite(hash);
            //TODO does not clear file but writes on top resulting in invalid jsons
            await JsonSerializer.SerializeAsync(writer, user);
        }

        public Task<IReadOnlyCollection<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<User>> GetAll(Expression<Func<User, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task Remove(string id)
        {
            throw new NotImplementedException();
        }

        public Task Update(User enitity)
        {
            throw new NotImplementedException();
        }

        public Task<User> Get(Expression<Func<User, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }

}
