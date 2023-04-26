using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPFClient.Model;
using WPFClient.Service.Dto;
using WPFClient.Store;

namespace WPFClient.TimeEditor.Service
{
    public class TimeService
    {
        public const string Url = "http://localhost:5271/user/time";

        private readonly HttpClient httpClient;
        private readonly ProfileStore profileStore;

        public TimeService(HttpClient httpClient, ProfileStore profileStore)
        {
            this.httpClient = httpClient;
            this.profileStore = profileStore;
        }

        public async Task DeleteTimeRange(TimeRangeModel timeRange)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, Url);
                var timeDto = new TimeWindowDto
                {
                    StartTime = (int)timeRange.StartTime.TotalMinutes,
                    EndTime = (int)timeRange.EndTime.TotalMinutes
                };

                request.Content = JsonContent.Create(timeDto);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                profileStore.PlayerModel.RemoveTime(timeRange);
            }
            catch (Exception e)
            {
                // TODO handle error
                Console.WriteLine(e.Message);
            }
        }

        public async Task AddTimeRange(TimeRangeModel timeRange)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, Url);
                var timeDto = new TimeWindowDto
                {
                    StartTime = (int)timeRange.StartTime.TotalMinutes,
                    EndTime = (int)timeRange.EndTime.TotalMinutes
                };

                request.Content = JsonContent.Create(timeDto);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                profileStore.PlayerModel.AddTime(timeRange);
            }
            catch (Exception e)
            {
                // TODO handle error
                Console.WriteLine(e.Message);
            }
        }
    }
}
