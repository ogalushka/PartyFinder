using System.Text.Json.Serialization;
//TODO group dtos by usage, database / client api / external api

namespace GamesCatalog.Dto
{
    public class GameDto{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CoverUrl { get; set; } = string.Empty;
    }
}
