using WPFClient.Model;

namespace WPFClient.Matches.Models
{
    public class PlayerMatchModel
    {
        public readonly string Id;
        public readonly string Name;
        public readonly GameModel[] GameModels;
        public readonly TimeRangeModel[] TimeRanges;
        public readonly string? DiscordId;

        public PlayerMatchModel(string id, string name, GameModel[] gameModels, TimeRangeModel[] timeRanges, string? discordId = null)
        {
            Id = id;
            Name = name;
            GameModels = gameModels;
            TimeRanges = timeRanges;
            DiscordId = discordId;
        }
    }
}
