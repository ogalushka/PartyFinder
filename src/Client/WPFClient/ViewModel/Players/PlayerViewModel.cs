using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using WPFClient.Matches.Models;

namespace WPFClient.ViewModel.Players
{
    public class PlayerViewModel : ViewModelBase
    {
        public PlayerViewModel(PlayerMatchModel model)
        {
            Name = model.Id;
            var sortedTimeRanges = model.TimeRanges.OrderByDescending(t => (t.EndTime - t.StartTime).TotalMinutes);

            //TODO max time ranges
            foreach (var timeRange in sortedTimeRanges)
            {
                MatchedTimes.Add(new TimeRangeViewModel(timeRange));
            }
            //MatchedTimes.Add(new TimeRangeViewModel("", "More...", "", ""));

            //TODO max Games
            foreach (var game in model.GameModels)
            {
                MatchedGames.Add(new GameMatchViewModel(game.Url, game.Name));
            }
        }

        public ObservableCollection<TimeRangeViewModel> MatchedTimes { get; } = new();
        public ObservableCollection<GameMatchViewModel> MatchedGames { get; } = new();
        #region props
        private string name = "";
        public string Name {
            get { return name; }
            set { SetField(ref name, value); }
        }

        #endregion
    }
}
