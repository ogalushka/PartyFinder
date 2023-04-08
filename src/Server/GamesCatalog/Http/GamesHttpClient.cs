﻿using GamesCatalog.Dto;
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

        public async Task GetGames()
        {
            var query = new Dictionary<string, string?>()
            {
                ["key"] = apiKey,
                ["tags"] = string.Join(",", DefaultTags)
            };

            var uri = QueryHelpers.AddQueryString("api/games", query);
            var result = await httpClient.GetAsync(uri);
            var str = await result.Content.ReadAsStringAsync();
            var parsed = await result.Content.ReadFromJsonAsync<GamesDto>();
            //var result = await httpClient.GetFromJsonAsync<GamesDto>(uri);
            //var resultStr = result.ToString();
            Console.WriteLine(str);
        }
    }
}