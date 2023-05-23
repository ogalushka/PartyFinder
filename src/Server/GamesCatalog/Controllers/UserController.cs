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

            Console.WriteLine("Getting game");
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
                Console.WriteLine("adding a game");
                dbContext.Games.Add(game);
            }

            Console.WriteLine("adding a game for a player");
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
        public async Task<IActionResult> AddTimeWindow([FromBody] TimeWindowDto timeWindow)
        {
            var player = new PlayerInfo { Id = GetUserGuid() };
            dbContext.PlayerInfo.Attach(player);
            // TODO validation
            dbContext.PlayerTimes.Add(new PlayerTime
            {
                PlayerInfo = player,
                StartTime = timeWindow.StartTime,
                EndTime = timeWindow.EndTime
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("time")]
        public async Task<IActionResult> RemoveTimeWindow([FromBody] TimeWindowDto timeWindow)
        {
            var player = new PlayerInfo { Id = GetUserGuid() };
            dbContext.PlayerInfo.Attach(player);
            dbContext.PlayerTimes.Remove(new PlayerTime
            {
                PlayerInfo = player,
                StartTime = timeWindow.StartTime,
                EndTime = timeWindow.EndTime
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetProfile(string? userId)
        {
            var requestedId = string.IsNullOrEmpty(userId) ? GetUserGuid() : Guid.Parse(userId);
            var player = await dbContext.PlayerInfo
                .Include(p => p.Games)
                .Include(p => p.Times)
                .FirstOrDefaultAsync(p => p.Id == requestedId);

            if (player == null)
            {
                return NotFound("Player not found");
            }

            var dto = new UserDto(
                   player.Games.Select(FromGame).ToArray(),
                   player.Times.Select(FromTime).ToArray()
                   );

            return Ok(dto);
        }

        [HttpGet]
        [Route("find2")]
        public async Task<ActionResult<PlayerMatchDto>> Find2()
        {
            // TODO add requests accepts
            var currentUserId = GetUserGuid();

            var playerData = dbContext.PlayerInfoGame
                .Join(dbContext.PlayerTimes,
                    pg => pg.PlayerInfoId,
                    pt => pt.PlayerInfoId,
                    (playerGame, playerTime) => new { playerGame, playerTime });

            var requests = await dbContext.Invitations
                .Where(i => i.SenderId == currentUserId || i.ReceiverId == currentUserId).ToListAsync();

            var matches = await playerData
                .Join(playerData,
                    pd1 => 1,
                    pd2 => 1,
                    (current, other) => new { current, other })
                .Where(r => r.current.playerGame.PlayerInfoId == currentUserId
                    && r.other.playerGame.PlayerInfoId != currentUserId
                    && r.current.playerGame.GameId == r.other.playerGame.GameId
                    && r.current.playerTime.StartTime < r.other.playerTime.EndTime
                    && r.current.playerTime.EndTime > r.other.playerTime.StartTime)
                .Select(r => r.other)
                .Join(dbContext.Games,
                    pi => pi.playerGame.GameId,
                    g => g.Id,
                    (r, game) => new { r.playerGame, r.playerTime, game })
                .Join(dbContext.PlayerInfo,
                    r => r.playerGame.PlayerInfoId,
                    pi => pi.Id,
                    (r, playerInfo) => new { playerInfo.Id, playerInfo, r.playerTime, r.game })
                .ToListAsync();

            var playerMatches = new Dictionary<Guid, PlayerMatchDto>();
            foreach (var match in matches)
            {
                if (playerMatches.TryGetValue(match.Id, out var player))
                {
                    if (!player.MatchingGames.Any(g => g.Id == match.game.Id))
                    {
                        player.MatchingGames.Add(FromGame(match.game));
                    }

                    if (!player.MatchingTimeWindows.Any(t => t.StartTime == match.playerTime.StartTime && t.EndTime == match.playerTime.EndTime))
                    {
                        player.MatchingTimeWindows.Add(FromTime(match.playerTime));
                    }
                }
                else
                {
                    playerMatches.Add(match.Id, new PlayerMatchDto(
                        match.Id.ToString(),
                        match.playerInfo.Name,
                        new List<GameDto> { FromGame(match.game) },
                        new List<TimeWindowDto> { FromTime(match.playerTime) }
                    ));
                }
            }

            var matched = new List<PlayerMatchDto>();
            var sent = new List<PlayerMatchDto>();
            var received = new List<PlayerMatchDto>();
            var accepted = new List<PlayerMatchDto>();

            foreach (var player in playerMatches.Values)
            {
                var otherPlayerGuid = Guid.Parse(player.PlayerId);
                var sentRequest = requests.Any(r => r.SenderId == currentUserId && r.ReceiverId == otherPlayerGuid);
                var receivedRequest = requests.Any(r => r.SenderId == otherPlayerGuid && r.ReceiverId == currentUserId);

                if (sentRequest && receivedRequest)
                {
                    accepted.Add(player);
                }
                else if (sentRequest)
                {
                    sent.Add(player);
                }
                else if (receivedRequest)
                {
                    received.Add(player);
                }
                else
                {
                    matched.Add(player);
                }
            }

            return Ok(new PlayerMatchesDto(matched, received, sent, accepted));
        }

        [HttpPost]
        [Route("accept")]
        public async Task<IActionResult> RequestMatch(string playerId)
        {
            dbContext.Invitations.Add(new Invitations
            {
                SenderId = GetUserGuid(),
                ReceiverId = Guid.Parse(playerId)
            });
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("cancel")]
        public async Task<IActionResult> RejectMatch(string playerId)
        {
            dbContext.Invitations.Remove(new Invitations
            {
                SenderId = GetUserGuid(),
                ReceiverId = Guid.Parse(playerId)
            });

            await dbContext.SaveChangesAsync();
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

        private GameDto FromGame(Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                CoverUrl = game.CoverUrl
            };
        }

        private TimeWindowDto FromTime(PlayerTime time)
        {
            return new TimeWindowDto
            {
                StartTime = time.StartTime,
                EndTime = time.EndTime
            };
        }
    }
}
