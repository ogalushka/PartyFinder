using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPFClient.Store;

namespace WPFClient.GameCatalog.Service
{
    public class GamesService
    {
        public const string ServiceUrl = "http://localhost:5271/games";

        private readonly HttpClient httpClient;
        private readonly SessionStore sessionStore;

        public GamesService(HttpClient httpClient, SessionStore sessionStore)
        {
            this.httpClient = httpClient;
            this.sessionStore = sessionStore;
        }

        public async Task<GameDto[]> GetGames(string? searchQuery)
        {
            string url = ServiceUrl;
            if (!string.IsNullOrEmpty(searchQuery))
            {
                url = ServiceUrl + $"?name={searchQuery}";
            }
            var result = await httpClient.GetAsync(url);
            var parsed = await result.Content.ReadFromJsonAsync<GameDto[]>();
            return parsed ?? throw new Exception("Can't parse games response");
        }
    }
}
