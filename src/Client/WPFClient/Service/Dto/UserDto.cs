using System;
using WPFClient.GameCatalog.Service;

namespace WPFClient.Service.Dto
{
    public class UserDto
    {
        public GameDto[] Games { get; set; } = Array.Empty<GameDto>();
        public TimeDto[] Times { get; set; } = Array.Empty<TimeDto>();
    }
}
