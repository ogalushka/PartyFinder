using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFClient.Infra;
using WPFClient.Matches.Models;

namespace WPFClient.ViewModel.Players
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly PlayerViewState viewState;

        public PlayerViewModel(PlayerMatchModel model, PlayerViewState viewState)
        {
            this.viewState = viewState;

            Id = model.Id;
            Name = model.Name;
            DiscordId = model.DiscordId ?? string.Empty;
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

        public string Id { get; set; }
        public string DiscordId { get; set; }

        public Visibility RequestButtonVisibility => IsRequestSendable() ? Visibility.Visible : Visibility.Collapsed;
        public Visibility RemoveButtonVisibility => IsRequestCancelable() ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AcceptButtonVisibility => IsRequestAcceptable() ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DiscordIdVisibility => viewState == PlayerViewState.Matched ? Visibility.Visible : Visibility.Collapsed;
        
        #endregion

        private bool IsRequestSendable()
        {
            return viewState == PlayerViewState.Found;
        }

        private bool IsRequestAcceptable()
        {
            return viewState == PlayerViewState.RequestReceived;
        }

        private bool IsRequestCancelable()
        {
            return viewState == PlayerViewState.RequestSent;
        }
    }

    public enum PlayerViewState
    {
        Found,
        RequestSent,
        RequestReceived,
        Matched
    }
}
