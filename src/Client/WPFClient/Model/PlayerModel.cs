namespace WPFClient.Model
{
    public class PlayerModel
    {
        public readonly string Name;
        public readonly GameModel[] Games;
        public readonly TimeRangeModel[] TimeRanges;

        public PlayerModel(string name, GameModel[] games, TimeRangeModel[] timeRanges)
        {
            Name = name;
            Games = games;
            TimeRanges = timeRanges;
        }
    }
}
