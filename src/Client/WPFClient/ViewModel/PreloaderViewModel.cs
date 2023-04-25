using System.Threading.Tasks;
using WPFClient.Service;
using WPFClient.Store;
using WPFClient.Views;

namespace WPFClient.ViewModel
{
    public class PreloaderViewModel : ViewModelBase
    {
        private readonly IdentityService identityService;
        private readonly Navigation navigation;

        public PreloaderViewModel(IdentityService identityService, Navigation navigation)
        {
            this.identityService = identityService;
            this.navigation = navigation;

            //TryGetUserLoggedInUser();
        }

        public async Task LoadApp()
        {
            var userLoggedIn = await identityService.GetUserData();
            if (userLoggedIn)
            {
                navigation.SetViewModel<HomePageViewModel>();
            }
            else
            {
                navigation.SetViewModel<LoginViewModel>();
            }
        }

        private async void TryGetUserLoggedInUser()
        {
            var userLoggedIn = await identityService.GetUserData();
            if (userLoggedIn)
            {
                navigation.SetViewModel<HomePageViewModel>();
            }
            else
            {
                navigation.SetViewModel<LoginViewModel>();
            }
        }
    }
}
