using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.Model;
using WPFClient.Store;
using WPFClient.TimeEditor.ViewModel;
using WPFClient.ViewModel.Players;

namespace WPFClient.ViewModel
{
    //TODO correct time view
    public class HomePageViewModel : ViewModelBase
    {
        private readonly User user;
        private readonly CommandFactory commandFactory;

        public HomePageViewModel(SessionStore sessionStore, CommandFactory commandFactory)
        {
            this.user = sessionStore.User;
            matchedPlayers = new();
            matchedPlayers.Add(new PlayerViewModel("name1"));
            this.commandFactory = commandFactory;
        }

        #region props
        private ObservableCollection<PlayerViewModel> matchedPlayers;
        public ObservableCollection<PlayerViewModel> MatchedPlayers => matchedPlayers;

        public BitmapImage Img => new BitmapImage(new Uri("https://media.rawg.io/media/games/456/456dea5e1c7e3cd07060c14e96612001.jpg"));
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
