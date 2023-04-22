using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPFClient.GameCatalog.Service;
using WPFClient.Model;
using WPFClient.Service.Dto;
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
            // TODO send in post??
            var uri = new Uri($"http://localhost:5189/login?username={email}&password={password}");
            // TODO catch
            var result = await httpClient.GetAsync(uri);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                //TODO check is user profile get is successfull
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
            // TODO continue here, get games from get all user data from GET /user
            // and also set time ranges
            var uri = new Uri("http://localhost:5271/user");
            var result = await httpClient.GetAsync(uri);
            var user = await result.Content.ReadFromJsonAsync<UserDto>();
            if (user == null)
            {
                return;
            }

            profileStore.PlayerModel.Games.AddRange(
                    user.Games.Select(g => 
                        new GameModel(g.Id, g.Name, g.CoverUrl)
                    )
                );
            profileStore.PlayerModel.TimeRanges.AddRange(
                    user.Times.Select(t =>
                        new TimeRangeModel(t.StartTime, t.EndTime)
                    )
                );
        }
    }
}
