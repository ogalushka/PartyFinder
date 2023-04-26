using WPFClient.Model;

namespace WPFClient.Matches.Models
{
    public class PlayerMatchModel
    {
        public readonly string Id;
        public readonly GameModel[] GameModels;
        public readonly TimeRangeModel[] TimeRanges;

        public PlayerMatchModel(string id, GameModel[] gameModels, TimeRangeModel[] timeRanges)
        {
            Id = id;
            GameModels = gameModels;
            TimeRanges = timeRanges;
        }
    }
}
