using GamesCatalog.Contract;
using GamesCatalog.Dto;
using Microsoft.AspNetCore.WebUtilities;

namespace GamesCatalog.Http
{
    // TODO error handling
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

        public async Task<GameDto[]> GetGames(string? name)
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
            var parsed = await result.Content.ReadFromJsonAsync<ContractGamesDto>();
            if (parsed == null)
            {
                return Array.Empty<GameDto>();
            }

            return parsed.Results.Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                CoverUrl = g.BackgroundImage
            }).ToArray();
        }

        public async Task<GameDto> GetGame(int gameId)
        {
            var query = new Dictionary<string, string?>()
            {
                ["key"] = apiKey,
            };
            var uri = QueryHelpers.AddQueryString($"api/games/{gameId}", query);
            var result = await httpClient.GetAsync(uri);
            var parsed = await result.Content.ReadFromJsonAsync<ContractGameDto>();
            if (parsed == null || parsed.Id < 0)
            {
                throw new ApplicationException($"Failed to get game with gameId: {gameId}");
            }

            return new GameDto
            {
                Id = parsed.Id,
                Name = parsed.Name,
                CoverUrl = parsed.BackgroundImage
            };
        }
    }
}
