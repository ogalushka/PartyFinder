using GamesCatalog.Dto;
using GamesCatalog.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesCatalog.Controllers
{
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UsersRepository usersRepository;

        public UserController(UsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        [Route("find")]
        public async Task<ActionResult<IEnumerable<UserMatchDto>>> Find()
        {
            var userList = await usersRepository.GetUserRecomendations(GetUserId());

            //TODO check sorting with database querry
            var result = userList.Order(Comparer<UserMatchDto>.Create(
                (u1, u2) => u1.MatchingGames.Count - u2.MatchingGames.Count)
            ); 
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile(string? userId)
        {
            var requestedId = string.IsNullOrEmpty(userId) ? GetUserId() : userId;
            //await usersRepository.GetGamesList(requestedId);
            return Ok();
        }

        [HttpPost]
        [Route("games")]
        public async Task<IActionResult> AddGame(int gameId)
        {
            await usersRepository.AddGame(GetUserId(), gameId);
            return Ok();
        }

        [HttpDelete]
        [Route("games")]
        public async Task<IActionResult> RemoveGame(int gameId)
        {
            await usersRepository.RemoveGame(GetUserId(), gameId);
            return Ok();
        }

        [HttpPost]
        [Route("time")]
        public async Task<IActionResult> AddTimeWindows([FromBody] TimeWindowDto timeWindow)
        {
            await usersRepository.AddTimeRange(GetUserId(), timeWindow);
            return Ok();
        }

        [HttpDelete]
        [Route("time")]
        public async Task<IActionResult> RemoveTimeWindows([FromBody] TimeWindowDto timeWindow)
        {
            await usersRepository.RemoteTimeRange(GetUserId(), timeWindow);
            return Ok();
        }

        private string GetUserId()
        {
            return HttpContext.User.Claims.First(kv => kv.Type == "username").Value;
        }
        
    }
}
