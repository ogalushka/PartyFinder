using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Matches.Service;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Matches.Command
{
    public class CancelMatchCommand : AsyncCommand
    {
        private readonly PlayerService playerService;
        private readonly HomePageViewModel homePageViewModel;
        private readonly ProfileStore profileStore;

        public CancelMatchCommand (PlayerService playerService, HomePageViewModel homePageViewModel, ProfileStore profileStore)
        {
            this.playerService = playerService;
            this.homePageViewModel = homePageViewModel;
            this.profileStore = profileStore;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if (parameter is string playerId && !string.IsNullOrEmpty(playerId))
            {
                var matchCanceled = await playerService.CancelMatch(playerId);
                if (matchCanceled)
                {
                    var sentRequests = profileStore.PlayerMatches.SentRequests;
                    var canceledMatch = sentRequests.Find(m => m.Id == playerId);
                    if (canceledMatch == null)
                    {
                        Debug.Assert(true, $"View and model desync, trying to Accept a user id:{playerId} match that doesnt exists in model PlayerMatches.FoundMatches");
                        return;
                    }

                    sentRequests.Remove(canceledMatch);
                    profileStore.PlayerMatches.FoundMatches.Add(canceledMatch);

                    homePageViewModel.RefreshLists();
                }
                else
                {
                    // TODO error handling
                    MessageBox.Show("Failed to reject match");
                }
            }
        }
    }
}
