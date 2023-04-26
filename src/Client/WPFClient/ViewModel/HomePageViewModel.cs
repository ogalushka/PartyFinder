using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.GameCatalog.ViewModel;
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

        public HomePageViewModel(SessionStore sessionStore, CommandFactory commandFactory, PlayerService playerService)
        {
            user = sessionStore.User;
            matchedPlayers = new();
            this.commandFactory = commandFactory;
            this.playerService = playerService;
            GetMatches();
        }

        private async void GetMatches()
        {
            matchedPlayers.Clear();
            var matches = await playerService.GetMatches();
            foreach (var match in matches)
            {
                matchedPlayers.Add(new PlayerViewModel(match));
            }
        }

        #region props
        private ObservableCollection<PlayerViewModel> matchedPlayers;
        public ObservableCollection<PlayerViewModel> MatchedPlayers => matchedPlayers;

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
    }
}
