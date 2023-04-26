namespace GamesCatalog.Dto
{
    public record PlayerMatchDto(string PlayerId, List<GameDto> MatchingGames, List<TimeWindowDto> MatchingTimeWindows); 
    public record UserDto(GameDto[] Games, TimeWindowDto[] Times);
}
