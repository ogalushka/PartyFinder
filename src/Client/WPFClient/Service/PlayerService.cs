using System.Net.Http;

namespace WPFClient.Service
{
    public class PlayerService
    {
        private readonly HttpClient httpClient;

        public PlayerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
    }
}
