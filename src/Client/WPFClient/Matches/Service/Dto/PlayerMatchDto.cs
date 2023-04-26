using WPFClient.GameCatalog.Service;
using WPFClient.Service.Dto;

namespace WPFClient.Matches.Service.Dto
{
    public record PlayerMatchDto(string PlayerId, GameDto[] MatchingGames, TimeWindowDto[] MatchingTimeWindows); 
}
