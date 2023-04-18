using System.Threading.Tasks;
using WPFClient.GameCatalog.Service;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.Model;

namespace WPFClient.GameCatalog.Command
{
    public class ToggleGameCommand : AsyncCommand
    {
        private readonly GamesService gamesService;
        private readonly GameCatalogGameItemViewModel gameItemViewModel;

        public ToggleGameCommand(GamesService gamesService, GameCatalogGameItemViewModel gameItemViewModel)
        {
            this.gamesService = gamesService;
            this.gameItemViewModel = gameItemViewModel;
        }

        protected override async Task ExecuteAsync()
        {
            if (gameItemViewModel.Added)
            {
                await gamesService.RemoveGame(gameItemViewModel.Id);
            }
            else
            {
                var gameModel = new GameModel(gameItemViewModel.Id, gameItemViewModel.Name, gameItemViewModel.CoverUrl);
                await gamesService.AddGame(gameModel);
            }

            gameItemViewModel.Added = !gameItemViewModel.Added;
        }
    }
}
