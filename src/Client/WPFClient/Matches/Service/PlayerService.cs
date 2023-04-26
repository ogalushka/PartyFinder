using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Matches.Models;
using WPFClient.Matches.Service.Dto;
using WPFClient.Model;

namespace WPFClient.Matches.Service
{
    public class PlayerService
    {
        private readonly HttpClient httpClient;

        public PlayerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PlayerMatchModel[]> GetMatches()
        {
            try
            {
                var result = await httpClient.GetAsync("http://localhost:5271/user/find");
                result.EnsureSuccessStatusCode();
                var parsed = await result.Content.ReadFromJsonAsync<PlayerMatchDto[]>();
                if (parsed == null)
                {
                    MessageBox.Show($"Failed to parse matches");
                    return Array.Empty<PlayerMatchModel>();
                }

                return parsed.Select(m => new PlayerMatchModel(
                    m.PlayerId,
                    m.MatchingGames.Select(g => new GameModel(g.Id, g.Name, g.CoverUrl)).ToArray(),
                    m.MatchingTimeWindows.Select(t => new TimeRangeModel(t.StartTime, t.EndTime)).ToArray()
                    )).ToArray();
            }
            catch (Exception e)
            {
                // TODO search for all MessageBox stuff and replace with better error handling
                MessageBox.Show($"Failed to get matches {e}");
                return Array.Empty<PlayerMatchModel>();
            }
        }
    }
}
