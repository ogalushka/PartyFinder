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
        public async Task<IActionResult> Get()
        {
            await gamesHttpClient.GetGames();
            return Ok();
        }
    }
}
