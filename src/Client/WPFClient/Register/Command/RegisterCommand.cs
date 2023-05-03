using System;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Register.ViewModel;
using WPFClient.Service;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Register.Command
{
    public class RegisterCommand : AsyncCommand
    {
        private readonly RegisterViewModel viewModel;
        private readonly IdentityService identityService;
        private readonly Navigation navigation;

        public RegisterCommand(RegisterViewModel viewModel, IdentityService identityService, Navigation navigation)
        {
            this.viewModel = viewModel;
            this.identityService = identityService;
            this.navigation = navigation;
            this.viewModel.PropertyChanged += (_, _) => InvokeCanExecute();
        }

        public override bool CanExecute(object? parameter)
        {
            // TODO data validation
            return base.CanExecute(parameter) 
                && !string.IsNullOrEmpty(viewModel.Email)
                && !string.IsNullOrEmpty(viewModel.Password)
                && !string.IsNullOrEmpty(viewModel.DiscordId)
                && !string.IsNullOrEmpty(viewModel.UserName)
                && viewModel.Password == viewModel.ConfirmPassword;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            viewModel.IsLoading = true;
            try
            {
                var registered = await identityService.TryRegister(viewModel.Email, viewModel.Password, viewModel.UserName, viewModel.DiscordId);
                if (registered)
                {
                    navigation.SetViewModel<HomePageViewModel>();
                }
                else
                {
                    // TODO add reasons
                    MessageBox.Show("Failed to register");
                }
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }
    }
}
