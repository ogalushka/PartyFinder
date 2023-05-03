using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
        private readonly HttpClientHandler httpClientHandler;
        private readonly SessionStore sessionStore;
        private readonly ProfileStore profileStore;
        private readonly PersistentStorage persistentStorage;

        public IdentityService(HttpClient httpClient, HttpClientHandler httpClientHandler, SessionStore sessionStore, ProfileStore profileStore, PersistentStorage persistentStorage)
        {
            this.httpClient = httpClient;
            this.httpClientHandler = httpClientHandler;
            this.sessionStore = sessionStore;
            this.profileStore = profileStore;
            this.persistentStorage = persistentStorage;
        }

        public async Task<bool> GetUserData()
        {
            var cookies = persistentStorage.GetCookies();
            if (cookies.Length == 0)
            {
                return false;
            }

            foreach (var cookie in cookies)
            {
                httpClientHandler.CookieContainer.Add(cookie);
            }

            var userProfileRetrieved = await GetUserProfile();
            if (!userProfileRetrieved)
            {
                persistentStorage.ClearCookies();
            }

            return userProfileRetrieved;
        }

        public async Task<bool> TryRegister(string email, string password, string username, string discrodId)
        {
            try
            {
                var uri = new Uri($"http://localhost:5189/register");
                var result = await httpClient.PostAsJsonAsync(uri, new RegistrationDto(username, email, password, discrodId));
                result.EnsureSuccessStatusCode();

                var loginCookies = httpClientHandler.CookieContainer.GetAllCookies();
                persistentStorage.SaveCookies(loginCookies);

                await GetUserProfile();
                return true;
            }
            catch (Exception)
            {
                // TODO display error
                return false;
            }
        }

        public async Task<bool> TryLogin(string email, string password)
        {
            try
            {
                var uri = new Uri($"http://localhost:5189/login?email={email}&password={password}");

                var result = await httpClient.GetAsync(uri);
                result.EnsureSuccessStatusCode();

                var loginCookies = httpClientHandler.CookieContainer.GetAllCookies();
                persistentStorage.SaveCookies(loginCookies);

                await GetUserProfile();
                return true;
            }
            catch(Exception)
            {
                // TODO display an error
                return false;
            }
        }

        private async Task<bool> GetUserProfile()
        {
            try
            {
                var uri = new Uri("http://localhost:5271/user");
                var result = await httpClient.GetAsync(uri);
                var user = await result.Content.ReadFromJsonAsync<UserDto>();
                if (user == null)
                {
                    return false;
                }

                profileStore.PlayerModel.Games.Clear();
                profileStore.PlayerModel.Games.AddRange(
                        user.Games.Select(g =>
                            new GameModel(g.Id, g.Name, g.CoverUrl)
                        )
                    );
                profileStore.PlayerModel.TimeRanges.Clear();
                profileStore.PlayerModel.TimeRanges.AddRange(
                        user.Times.Select(t =>
                            new TimeRangeModel(t.StartTime, t.EndTime)
                        )
                    );
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
