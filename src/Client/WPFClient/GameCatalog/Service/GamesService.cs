using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPFClient.Model;
using WPFClient.Store;

namespace WPFClient.GameCatalog.Service
{
    public class GamesService
    {
        public const string GetGamesUrl = "http://localhost:5271/games?name={0}";
        public const string GameUrl = "http://localhost:5271/user/games?gameId={0}";

        private readonly HttpClient httpClient;
        private readonly ProfileStore profileStore;

        public GamesService(HttpClient httpClient, ProfileStore profileStore)
        {
            this.httpClient = httpClient;
            this.profileStore = profileStore;
        }

        public async Task<GameDto[]> GetGames(string? searchQuery)
        {
            string url = string.Format(GetGamesUrl, searchQuery);
            var result = await httpClient.GetAsync(url);
            var parsed = await result.Content.ReadFromJsonAsync<GameDto[]>();
            if (parsed == null)
            {
                return Array.Empty<GameDto>();
            }
            return parsed;
        }

        public async Task AddGame(GameModel gameModel)
        {
            string url = string.Format(GameUrl, gameModel.Id);
            var result = await httpClient.PostAsync(url, null);
            result.EnsureSuccessStatusCode();
            profileStore.PlayerModel.AddGame(gameModel);
        }

        public async Task RemoveGame(int gameId)
        {
            string url = string.Format(GameUrl, gameId);
            var result = await httpClient.DeleteAsync(url);
            result.EnsureSuccessStatusCode();
            profileStore.PlayerModel.RemoveGame(gameId);
        }
    }
}
