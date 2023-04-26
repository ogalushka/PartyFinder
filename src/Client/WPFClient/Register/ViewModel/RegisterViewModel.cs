using System.Windows.Input;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.Infra;
using WPFClient.Register.Command;
using WPFClient.ViewModel;

namespace WPFClient.Register.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        // TODO errors
        private readonly CommandFactory commandFactory;
        public RegisterViewModel(CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        private bool isLoading = false;
        public bool IsLoading {
            get { return isLoading; }
            set { SetField(ref isLoading, value); }
        }

        private string email = "";
        public string Email {
            get { return email; }
            set { SetField(ref email, value); }
        }

        private string password = "";
        public string Password {
            get { return password; }
            set { SetField(ref password, value); }
        }

        private string confirmPassword = "";
        public string ConfirmPassword {
            get { return confirmPassword; }
            set { SetField(ref confirmPassword, value); }
        }

        public ICommand RegisterCommand => commandFactory.Get<RegisterCommand>(this);
        public ICommand NavigateLoginCommand => commandFactory.Get<NavigateCommand<LoginViewModel>>();
    }
}
