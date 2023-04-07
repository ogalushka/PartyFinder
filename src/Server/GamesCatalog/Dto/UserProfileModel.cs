namespace GamesCatalog.Dto
{
    public record UserProfileDto(string userName, GameDto[] games, TimeWindowDto[] timeWindows);
}
