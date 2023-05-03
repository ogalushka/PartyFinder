using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Matches.Service;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Matches
{
    public class AcceptMatchCommand : AsyncCommand
    {
        private readonly PlayerService playerService;
        private readonly HomePageViewModel homePageViewModel;
        private readonly ProfileStore profileStore;

        public AcceptMatchCommand(PlayerService playerService, HomePageViewModel homePageViewModel, ProfileStore profileStore)
        {
            this.playerService = playerService;
            this.homePageViewModel = homePageViewModel;
            this.profileStore = profileStore;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if (parameter is string playerId && !string.IsNullOrEmpty(playerId))
            {
                var matchAccepted = await playerService.RequestMatch(playerId);
                if (matchAccepted)
                {
                    var receivedMatches = profileStore.PlayerMatches.ReceivedRequests;
                    var acceptedMatch = receivedMatches.Find(m => m.Id == playerId);
                    if (acceptedMatch == null)
                    {
                        Debug.Assert(true, $"View and model desync, trying to Accept a user id:{playerId} match that doesnt exists in model PlayerMatches.FoundMatches");
                        return;
                    }

                    receivedMatches.Remove(acceptedMatch);
                    profileStore.PlayerMatches.AcceptedRequests.Add(acceptedMatch);

                    homePageViewModel.RefreshLists();
                }
                else
                {
                    // TODO error handling
                    MessageBox.Show("Failed to accept match");
                }
            }
        }
    }
}
