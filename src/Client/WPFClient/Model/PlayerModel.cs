using System.Collections.Generic;
using System.Linq;

namespace WPFClient.Model
{
    public class PlayerModel
    {
        public readonly string Name;
        public readonly List<GameModel> Games;
        public readonly List<TimeRangeModel> TimeRanges;

        public PlayerModel(string name, List<GameModel> games, List<TimeRangeModel> timeRanges)
        {
            Name = name;
            Games = games;
            TimeRanges = timeRanges;
        }

        public void AddGame(GameModel game)
        {
            if (Games.Any(g => g.Id == game.Id))
            {
                return;
            }
            Games.Add(game);
        }

        public void RemoveGame(int gameId)
        {
            var index = Games.FindIndex(g => g.Id == gameId);
            if (index >= 0)
            {
                Games.RemoveAt(index);
            }
        }
    }
}
