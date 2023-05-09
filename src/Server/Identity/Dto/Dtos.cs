namespace Identity.Dto
{
    public record RegistrationDto(string UserName, string Email, string Password, string DiscordId);
    public record LoginDto(string Email, string Password);
}
