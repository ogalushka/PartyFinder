using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.Infra;
using WPFClient.Matches;
using WPFClient.Matches.Command;
using WPFClient.Matches.Service;
using WPFClient.Model;
using WPFClient.Store;
using WPFClient.TimeEditor.ViewModel;
using WPFClient.ViewModel.Players;

namespace WPFClient.ViewModel
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly User user;
        private readonly CommandFactory commandFactory;
        private readonly PlayerService playerService;
        private readonly ProfileStore profileStore;

        public HomePageViewModel(SessionStore sessionStore, CommandFactory commandFactory, PlayerService playerService, ProfileStore profileStore)
        {
            user = sessionStore.User;
            MatchedPlayers = new();
            SentRequests = new();
            ReceivedRequests = new();
            AcceptedRequests = new();
            this.commandFactory = commandFactory;
            this.playerService = playerService;
            this.profileStore = profileStore;
            GetMatches();
        }

        private async void GetMatches()
        {
            MatchedPlayers.Clear();
            await playerService.GetMatches();
            RefreshLists();
        }

        // TODO performance??
        public void RefreshLists()
        {
            MatchedPlayers.Clear();
            SentRequests.Clear();
            ReceivedRequests.Clear();
            AcceptedRequests.Clear();

            foreach (var match in profileStore.PlayerMatches.FoundMatches)
            {
                MatchedPlayers.Add(new PlayerViewModel(match, PlayerViewState.Found));
            }
            foreach (var match in profileStore.PlayerMatches.SentRequests)
            {
                SentRequests.Add(new PlayerViewModel(match, PlayerViewState.RequestSent));
            }
            foreach (var match in profileStore.PlayerMatches.ReceivedRequests)
            {
                ReceivedRequests.Add(new PlayerViewModel(match, PlayerViewState.RequestReceived));
            }
            foreach (var match in profileStore.PlayerMatches.AcceptedRequests)
            {
                AcceptedRequests.Add(new PlayerViewModel(match, PlayerViewState.Matched));
            }
        }

        #region props
        public ObservableCollection<PlayerViewModel> MatchedPlayers { get; private set; } 
        public ObservableCollection<PlayerViewModel> SentRequests { get; private set; } 
        public ObservableCollection<PlayerViewModel> ReceivedRequests { get; private set; } 
        public ObservableCollection<PlayerViewModel> AcceptedRequests { get; private set; } 

        public BitmapImage Img => new();
        //TODO setting directly to model in MVVM? This can also be readonly on this view
        public string Email {
            get {
                return user.Email;
            }
            set {
                SetField(ref user.Email, value);
            }
        }
        #endregion

        public ICommand LogOut {
            get {
                return commandFactory.Get<LogOutCommand>();
            }
        }

        public ICommand MyGames {
            get {
                return commandFactory.Get<NavigateCommand<GameCatalogViewModel>>();
            }
        }

        public ICommand MyTimes {
            get {
                return commandFactory.Get<NavigateCommand<TimeEditorViewModel>>();
            }
        }

        public ICommand RequestMatch => commandFactory.Get<SendMatchRequestCommand>(this);
        public ICommand AcceptMatch => commandFactory.Get<AcceptMatchCommand>(this);
        public ICommand CancelMatch => commandFactory.Get<CancelMatchCommand>(this);
        public ICommand CopyDiscordId => new DelegateCommand<string>((discordId) => Clipboard.SetText(discordId));
    }
}
