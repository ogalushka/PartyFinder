using WPFClient.GameCatalog.Service;
using WPFClient.Service.Dto;

namespace WPFClient.Matches.Service.Dto
{
    public record PlayerMatchesDto(PlayerMatchDto[] FoundMatches, PlayerMatchDto[] ReceivedRequests, PlayerMatchDto[] SentRequests, PlayerMatchDto[] AcceptedRequests);
    public record PlayerContactsDto(string DiscordId);
    public record PlayerMatchDto(string PlayerId, string Name, GameDto[] MatchingGames, TimeWindowDto[] MatchingTimeWindows, PlayerContactsDto? PlayerContacts); 
}
