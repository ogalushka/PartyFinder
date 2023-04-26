using System;
using System.Linq;
using System.Threading.Tasks;
using WPFClient.Factory;
using WPFClient.GameCatalog.Service;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.Store;

namespace WPFClient.GameCatalog.Command
{
    public class SearchGamesCommand : AsyncCommand
    {
        private readonly CommandFactory commandFactory;
        private readonly GameCatalogViewModel viewModel;
        private readonly GamesService gamesService;
        private readonly ProfileStore profileStore;

        public SearchGamesCommand(CommandFactory commandFactory, GameCatalogViewModel viewModel, GamesService gamesService, ProfileStore profileStore)
        {
            this.commandFactory = commandFactory;
            this.viewModel = viewModel;
            this.gamesService = gamesService;
            this.profileStore = profileStore;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            // TODO set loading
            if (string.IsNullOrEmpty(viewModel.SearchQuery))
            {
                var gamesViewModels = profileStore.PlayerModel.Games.Select(g =>
                    new GameCatalogGameItemViewModel(commandFactory, g, true));
                viewModel.SetGames(gamesViewModels);
            }
            else
            {
                var games = await gamesService.GetGames(viewModel.SearchQuery);
                viewModel.SetGames(games.Select(g =>
                    new GameCatalogGameItemViewModel(commandFactory, g.Id, g.Name, g.CoverUrl, profileStore.PlayerModel.Games.Any(pg => pg.Id == g.Id))
                    ));
            }
        }
    }
}
