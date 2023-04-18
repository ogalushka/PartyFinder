using System;
using System.Linq;
using System.Windows.Input;
using WPFClient.Factory;
using WPFClient.GameCatalog.Service;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.Store;

namespace WPFClient.GameCatalog.Command
{
    public class SearchGamesCommand : ICommand
    {
        private readonly CommandFactory commandFactory;
        private readonly GameCatalogViewModel viewModel;
        private readonly GamesService gamesService;
        private readonly ProfileStore profileStore;

        public event EventHandler? CanExecuteChanged;

        public SearchGamesCommand(CommandFactory commandFactory, GameCatalogViewModel viewModel, GamesService gamesService, ProfileStore profileStore)
        {
            this.commandFactory = commandFactory;
            this.viewModel = viewModel;
            this.gamesService = gamesService;
            this.profileStore = profileStore;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            // TODO set loading
            var games = await gamesService.GetGames(viewModel.SearchQuery);
            viewModel.SetGames(games.Select(g => 
                new GameCatalogGameItemViewModel(commandFactory, g.Id, g.Name, g.CoverUrl, profileStore.PlayerModel.Games.Any(pg => pg.Id == g.Id))
                ));
        }
    }
}
