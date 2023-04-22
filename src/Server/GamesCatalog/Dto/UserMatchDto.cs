﻿namespace GamesCatalog.Dto
{
    public record UserMatchDto(string PlayerId, HashSet<int> MatchingGames, List<TimeWindowDto> MatchingTimeWindows); 
    public record UserDto(GameDto[] Games, TimeWindowDto[] Times);
}
