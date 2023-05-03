using GamesCatalog.Repository;
using Identity.Contracts;
using MassTransit;

namespace GamesCatalog.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegistered>
    {
        private readonly UsersRepository repository;

        public UserRegisteredConsumer(UsersRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<UserRegistered> context)
        {
            var userData = context.Message;

            // TODO checkout db integration with GUID type
            await repository.AddUserContacts(userData.Id.ToString(), userData.Name, userData.Discord);
        }
    }
}
