using GamesCatalog.Dto;
using GamesCatalog.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamesCatalog.Controllers
{
    [ApiController]
    [Route("games")]
    public class GamesCatalogController : ControllerBase
    {
        private readonly GamesHttpClient gamesHttpClient;

        public GamesCatalogController(GamesHttpClient gamesHttpClient)
        {
            this.gamesHttpClient = gamesHttpClient;
        }

        [HttpGet]
        public async Task<ActionResult<GameDto[]>> Get(string? name)
        {
            var game = await gamesHttpClient.GetGames(name);
            return Ok(game.Results);
        }
    }
}
