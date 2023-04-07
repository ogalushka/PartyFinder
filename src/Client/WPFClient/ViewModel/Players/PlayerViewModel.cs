using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace WPFClient.ViewModel.Players
{
    public class PlayerViewModel : ViewModelBase
    {
        public PlayerViewModel(string name)
        {
            Name = name;
            MatchedTimes.Add(new TimeRangeViewModel("Mon", "17:00", "18:00"));
            MatchedTimes.Add(new TimeRangeViewModel("Tue", "10:00", "12:00"));
            MatchedTimes.Add(new TimeRangeViewModel("", "More...", ""));
            MatchedGames.Add(new BitmapImage(new Uri("https://media.rawg.io/media/games/456/456dea5e1c7e3cd07060c14e96612001.jpg")));
        }

        public ObservableCollection<TimeRangeViewModel> MatchedTimes { get; } = new();
        public ObservableCollection<BitmapImage> MatchedGames { get; } = new();
        #region props
        private string name = "";
        public string Name {
            get { return name; }
            set { SetField(ref name, value); }
        }

        #endregion
    }
}
