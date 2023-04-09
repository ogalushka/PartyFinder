using System;
using System.Linq;
using System.Windows.Input;
using WPFClient.GameCatalog.Service;
using WPFClient.GameCatalog.ViewModel;

namespace WPFClient.GameCatalog.Command
{
    public class SearchGamesCommand : ICommand
    {
        private readonly GameCatalogViewModel viewModel;
        private readonly GamesService gamesService;

        public event EventHandler? CanExecuteChanged;

        public SearchGamesCommand(GameCatalogViewModel viewModel, GamesService gamesService)
        {
            this.viewModel = viewModel;
            this.gamesService = gamesService;
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
                new GameCatalogGameItemViewModel(g.Name, g.BackgroundImage, false)
                ));
        }
    }
}
