using GamesCatalog.Database;
using Identity.Contracts;
using MassTransit;

namespace GamesCatalog.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegistered>
    {
        private readonly PlayersDbContext playersDbContext;

        public UserRegisteredConsumer(PlayersDbContext playersDbContext)
        {
            this.playersDbContext = playersDbContext;
        }

        public Task Consume(ConsumeContext<UserRegistered> context)
        {
            var userData = context.Message;
            playersDbContext.PlayerInfo.Add(new PlayerInfo
            {
                Id = userData.Id,
                Name = userData.Name,
                DiscordId = userData.Discord
            });
            return playersDbContext.SaveChangesAsync();
        }
    }
}
