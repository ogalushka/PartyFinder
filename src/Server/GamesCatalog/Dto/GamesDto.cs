﻿using System.Text.Json.Serialization;

namespace GamesCatalog.Dto
{
    public class GamesDto
    {
        public GameDto[] Results { get; set; } = Array.Empty<GameDto>();
    }

    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [JsonPropertyName("background_image")]
        public string BackgroundImage { get; set; } = "";
    }
}
