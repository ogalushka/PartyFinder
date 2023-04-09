using System.Text.Json.Serialization;

namespace WPFClient.GameCatalog.Service
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [JsonPropertyName("background_image")]
        public string BackgroundImage { get; set; } = "";
    }
}
