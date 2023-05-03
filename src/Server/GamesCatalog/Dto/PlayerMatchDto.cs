namespace GamesCatalog.Dto
{
    public record PlayerMatchesDto(IEnumerable<PlayerMatchDto> FoundMatches, IEnumerable<PlayerMatchDto> ReceivedRequests, IEnumerable<PlayerMatchDto> SentRequests, IEnumerable<PlayerMatchDto> AcceptedRequests);
    public record PlayerMatchDto(string PlayerId, string Name, List<GameDto> MatchingGames, List<TimeWindowDto> MatchingTimeWindows, PlayerContacts? PlayerContacts = null); 
    public record PlayerContacts(string DiscordId);
    public record UserDto(GameDto[] Games, TimeWindowDto[] Times);
}
