using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFClient.Service;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Command
{
    //TODO continue here: Save session between closing app
    public class LoginCommand : ICommand
    {
        private readonly LoginViewModel viewModel;
        private readonly Navigation navigationStore;
        private readonly IdentityService identityService;

        public event EventHandler? CanExecuteChanged;

        public LoginCommand(LoginViewModel viewModel, Navigation navigationStore, IdentityService identityService)
        {
            this.viewModel = viewModel;
            this.navigationStore = navigationStore;
            this.identityService = identityService;
            this.viewModel.PropertyChanged += (_, _) => CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object? parameter)
        {
            //TODO propper validation
            return viewModel.Email != "" && viewModel.Password.Length > 0;
        }

        public async void Execute(object? parameter)
        {
            try
            {
                viewModel.IsLoading = true;
                await Login();
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }

        private async Task Login()
        {
            //TODO handle failed login
            var loginSuccess = await identityService.TryLogin(viewModel.Email, viewModel.Password);
            if (loginSuccess)
            {
                navigationStore.SetViewModel<HomePageViewModel>();
                //navigationStore.SetViewModel<GameCatalogViewModel>();
            }
        }
    }
}
