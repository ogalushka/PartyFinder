//TODO naming??
using System.Text.Json.Serialization;

namespace GamesCatalog.Contract
{
    public class ContractGamesDto
    {
        public ContractGameDto[] Results { get; set; } = Array.Empty<ContractGameDto>();
    }

    public class ContractGameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [JsonPropertyName("background_image")]
        public string BackgroundImage { get; set; } = "";
    }
}
