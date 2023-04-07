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
    }
}
