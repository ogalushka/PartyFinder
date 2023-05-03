using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Matches.Models;
using WPFClient.Matches.Service.Dto;
using WPFClient.Model;
using WPFClient.Store;

namespace WPFClient.Matches.Service
{
    public class PlayerService
    {
        private readonly HttpClient httpClient;
        private readonly ProfileStore profileStore;

        public PlayerService(HttpClient httpClient, ProfileStore profileStore)
        {
            this.httpClient = httpClient;
            this.profileStore = profileStore;
        }

        public async Task<PlayerMatchesModel> GetMatches()
        {
            try
            {
                var result = await httpClient.GetAsync("http://localhost:5271/user/find");
                result.EnsureSuccessStatusCode();
                var parsed = await result.Content.ReadFromJsonAsync<PlayerMatchesDto>();
                if (parsed == null)
                {
                    MessageBox.Show($"Failed to parse matches");
                    return new PlayerMatchesModel();
                }

                profileStore.PlayerMatches = new PlayerMatchesModel(
                    ParseMatchDto(parsed.FoundMatches),
                    ParseMatchDto(parsed.ReceivedRequests),
                    ParseMatchDto(parsed.SentRequests),
                    ParseMatchDto(parsed.AcceptedRequests)
                    );

                return profileStore.PlayerMatches;
            }
            catch (Exception e)
            {
                // TODO search for all MessageBox stuff and replace with better error handling
                MessageBox.Show($"Failed to get matches {e}");
                return new PlayerMatchesModel();
            }
        }

        public async Task<bool> RequestMatch(string playerId)
        {
            try
            {
                var result = await httpClient.PostAsync($"http://localhost:5271/user/accept?playerId={playerId}", null);
                result.EnsureSuccessStatusCode();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelMatch(string playerId)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"http://localhost:5271/user/cancel?playerId={playerId}");
                result.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<PlayerMatchModel> ParseMatchDto(PlayerMatchDto[] dtos)
        {
            return dtos.Select(m => new PlayerMatchModel(
                    m.PlayerId,
                    m.Name,
                    m.MatchingGames.Select(g => new GameModel(g.Id, g.Name, g.CoverUrl)).ToArray(),
                    m.MatchingTimeWindows.Select(t => new TimeRangeModel(t.StartTime, t.EndTime)).ToArray(),
                    m.PlayerContacts?.DiscordId
                    )).ToList();
        }
    }
}
