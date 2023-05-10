using GamesCatalog.Database;
using GamesCatalog.Dto;
using GamesCatalog.Http;
using GamesCatalog.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GamesCatalog.Controllers
{
    //TODO validation
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UsersRepository usersRepository;
        private readonly GamesHttpClient gamesClient;
        private readonly PlayersDbContext dbContext;

        public UserController(UsersRepository usersRepository, GamesHttpClient gamesClient, PlayersDbContext dbContext)
        {
            this.usersRepository = usersRepository;
            this.gamesClient = gamesClient;
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("find")]
        public async Task<ActionResult<IEnumerable<PlayerMatchDto>>> Find()
        {
            Console.WriteLine("in find");
            var currentUser = GetUserId();
            var requestedIdsList = await usersRepository.GetRequestedPlayers(currentUser);
            var receivedIdsList = await usersRepository.GetReceivedInvitations(currentUser);
            var playerMatches = await usersRepository.GetUserRecomendations(currentUser);

            var foundMatches = new List<PlayerMatchDto>();
            var sentRequests = new List<PlayerMatchDto>();
            var receivedRequets = new List<PlayerMatchDto>();
            var acceptedRequests = new List<PlayerMatchDto>();

            foreach (var match in playerMatches)
            {
                var inSent = requestedIdsList.Contains(match.PlayerId);
                var inReceived = receivedIdsList.Contains(match.PlayerId);

                if (inSent && inReceived)
                {
                    acceptedRequests.Add(match);
                }
                else if (inSent)
                {
                    // TODO organize data better, so there is no need to delete contact info from request;
                    sentRequests.Add(new PlayerMatchDto(match.PlayerId, match.Name, match.MatchingGames, match.MatchingTimeWindows));
                }
                else if (inReceived)
                {
                    receivedRequets.Add(new PlayerMatchDto(match.PlayerId, match.Name, match.MatchingGames, match.MatchingTimeWindows));
                }
                else
                {
                    foundMatches.Add(new PlayerMatchDto(match.PlayerId, match.Name, match.MatchingGames, match.MatchingTimeWindows));
                }
            }

            //TODO check sorting with database querry
            var sortedFoundMatches = foundMatches.Order(Comparer<PlayerMatchDto>.Create(
                (u1, u2) => u1.MatchingGames.Count - u2.MatchingGames.Count)
            );

            var result = new PlayerMatchesDto(
                sortedFoundMatches,
                receivedRequets,
                sentRequests,
                acceptedRequests);

            return Ok(result);
        }

        [HttpPost]
        [Route("accept")]
        public async Task<IActionResult> RequestMatch(string playerId)
        {
            await usersRepository.AddMatchRequest(GetUserId(), playerId);
            return Ok();
        }

        [HttpDelete]
        [Route("cancel")]
        public async Task<IActionResult> RejectMatch(string playerId)
        {
            await usersRepository.RemoveMatchRequest(GetUserId(), playerId);
            return Ok();
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
        public async Task<ActionResult<List<GameDto>>> GetGames()
        {
            var games = await dbContext.PlayerInfoGame
                .Where(pg => pg.PlayerInfoId == GetUserGuid())
                .Join(
                    dbContext.Games,
                    pg => pg.GameId,
                    g => g.Id,
                    (pg, g) => new GameDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        CoverUrl = g.CoverUrl
                    })
                .ToListAsync();

            return Ok(games);
        }

        [HttpPost]
        [Route("games")]
        public async Task<IActionResult> AddGame(int gameId)
        {
            // TODO add recache if record is old
            var game = await dbContext.Games.SingleOrDefaultAsync(g => g.Id == gameId);
            var player = new PlayerInfo { Id = GetUserGuid() };
            dbContext.PlayerInfo.Attach(player);

            if (game == null)
            {
                var newGame = await gamesClient.GetGame(gameId);
                game = new Game 
                { 
                    Id = newGame.Id,
                    Name = newGame.Name,
                    CoverUrl = newGame.CoverUrl
                };
                dbContext.Games.Add(game);
            }

            player.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("games")]
        public async Task<IActionResult> RemoveGame(int gameId)
        {
            dbContext.PlayerInfoGame.Remove(new PlayerInfoGame { PlayerInfoId = GetUserGuid(), GameId = gameId });
            await dbContext.SaveChangesAsync();
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
            return HttpContext.User.Claims.First(kv => kv.Type == ClaimTypes.NameIdentifier).Value;
        }

        private Guid GetUserGuid()
        {
            return Guid.Parse(HttpContext.User.Claims.First(kv => kv.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
