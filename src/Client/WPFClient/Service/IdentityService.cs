using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WPFClient.Store;

namespace WPFClient.Service
{
    public class IdentityService
    {
        private readonly HttpClient httpClient;
        private readonly SessionStore sessionStore;

        public IdentityService(HttpClient httpClient, SessionStore sessionStore)
        {
            this.httpClient = httpClient;
            this.sessionStore = sessionStore;
        }

        public async Task<bool> TryLogin(string email, string password)
        {
            var uri = new Uri($"http://localhost:5189/login?username={email}&password={password}");
            // TODO catch
            var result = await httpClient.GetAsync(uri);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                if (result.Headers.TryGetValues("set-cookie", out var cookie))
                {
                    sessionStore.User.Email = email;
                    // TODO cookie handling check out, Application.setCookie and CookieContainer
                    sessionStore.User.Cookie = cookie.First();
                    return true;
                }
                return false;
            }
            else
            {
                // TODO display an error
                return false;
            }
        }
    }
}
