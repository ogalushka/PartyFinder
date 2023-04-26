using System;
using WPFClient.GameCatalog.Service;

namespace WPFClient.Service.Dto
{
    public class UserDto
    {
        public GameDto[] Games { get; set; } = Array.Empty<GameDto>();
        public TimeWindowDto[] Times { get; set; } = Array.Empty<TimeWindowDto>();
    }
}
