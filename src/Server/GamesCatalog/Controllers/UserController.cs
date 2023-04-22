using GamesCatalog.Dto;
using GamesCatalog.Http;
using GamesCatalog.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesCatalog.Controllers
{
    //TODO validation
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UsersRepository usersRepository;
        private readonly GamesHttpClient gamesClient;

        public UserController(UsersRepository usersRepository, GamesHttpClient gamesClient)
        {
            this.usersRepository = usersRepository;
            this.gamesClient = gamesClient;
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
        public async Task<ActionResult<UserDto>> GetProfile(string? userId)
        {
            var requestedId = string.IsNullOrEmpty(userId) ? GetUserId() : userId;
            var gamesTask = usersRepository.GetGames(requestedId);
            var timesTask = usersRepository.GetTimes(requestedId);

            var games = await gamesTask;
            var times = await timesTask;

            return Ok(new UserDto(games, times));
        }


        [HttpGet]
        [Route("games")]
        public async Task<ActionResult<GameDto[]>> GetGames()
        {
            return await usersRepository.GetGames(GetUserId());
        }

        [HttpPost]
        [Route("games")]
        public async Task<IActionResult> AddGame(int gameId)
        {
            var isGameCached = await usersRepository.GameCached(gameId);
            if (isGameCached)
            {
                await usersRepository.AddGame(GetUserId(), gameId);
            }
            else
            {
                var games = await gamesClient.GetGame(gameId);
                await usersRepository.AddGame(GetUserId(), gameId, games.Name, games.CoverUrl);
            }
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
            // TODO validation
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
