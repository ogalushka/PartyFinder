using System.Text.Json.Serialization;

namespace WPFClient.GameCatalog.Service
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? CoverUrl { get; set; } 
    }
}
