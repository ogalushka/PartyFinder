using GamesCatalog.Dto;
using Microsoft.AspNetCore.WebUtilities;

namespace GamesCatalog.Http
{
    public class GamesHttpClient
    {
        private static string[] DefaultTags = new [] { "multiplayer", "co-op" };

        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public GamesHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            apiKey = configuration.GetValue<string>("RAWGKey") ?? throw new Exception("No RAWGKey api key set");
        }

        public async Task<GamesDto> GetGames(string? name)
        {
            //TODO caching?
            var query = new Dictionary<string, string?>()
            {
                ["key"] = apiKey,
                ["tags"] = string.Join(",", DefaultTags),
                ["search"] = name
            };

            var uri = QueryHelpers.AddQueryString("api/games", query);
            var result = await httpClient.GetAsync(uri);
            var parsed = await result.Content.ReadFromJsonAsync<GamesDto>();
            return parsed ?? throw new ApplicationException("Failed to get games list");
        }
    }
}
