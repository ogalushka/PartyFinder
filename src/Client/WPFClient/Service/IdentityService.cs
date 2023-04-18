using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPFClient.GameCatalog.Service;
using WPFClient.Model;
using WPFClient.Store;

namespace WPFClient.Service
{
    public class IdentityService
    {
        private readonly HttpClient httpClient;
        private readonly SessionStore sessionStore;
        private readonly ProfileStore profileStore;

        public IdentityService(HttpClient httpClient, SessionStore sessionStore, ProfileStore profileStore)
        {
            this.httpClient = httpClient;
            this.sessionStore = sessionStore;
            this.profileStore = profileStore;
        }

        public async Task<bool> TryLogin(string email, string password)
        {
            var uri = new Uri($"http://localhost:5189/login?username={email}&password={password}");
            // TODO catch
            var result = await httpClient.GetAsync(uri);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                await GetUserProfile();
                sessionStore.User.Email = email;
                return true;
            }
            else
            {
                // TODO display an error
                return false;
            }
        }

        private async Task GetUserProfile()
        {
            var uri = new Uri("http://localhost:5271/user/games");
            var result = await httpClient.GetAsync(uri);
            var games = await result.Content.ReadFromJsonAsync<GameDto[]>();
            if (games == null)
            {
                return;
            }

            profileStore.PlayerModel.Games.AddRange(games.Select(g=> new GameModel(g.Id, g.Name, g.CoverUrl)));
        }
    }
}
