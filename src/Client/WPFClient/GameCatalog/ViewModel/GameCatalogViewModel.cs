using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.GameCatalog.Command;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.GameCatalog.ViewModel
{
    public class GameCatalogViewModel : ViewModelBase
    {
        private readonly CommandFactory commandFactory;
        public GameCatalogViewModel(CommandFactory commandFactory, ProfileStore profileStore)
        {
            Games = new();
            this.commandFactory = commandFactory;
            var myGames = profileStore.PlayerModel.Games.Select(g => new GameCatalogGameItemViewModel(commandFactory, g, true));
            SetGames(myGames);
        }

        public void SetGames(IEnumerable<GameCatalogGameItemViewModel> newGames)
        {
            //TODO add scroll
            Games.Clear();
            foreach (var game in newGames)
            {
                Games.Add(game);
            }
        }

        public ObservableCollection<GameCatalogGameItemViewModel> Games { get; }

        private string searchQuery = "";
        public string SearchQuery {
            get { return searchQuery; }
            set { SetField(ref searchQuery, value); }
        }

        // TODO live results update
        public ICommand SearchCommand => commandFactory.Get<SearchGamesCommand>(this);
        public ICommand NavigateHome => commandFactory.Get<NavigateCommand<HomePageViewModel>>(this);
    }
}
