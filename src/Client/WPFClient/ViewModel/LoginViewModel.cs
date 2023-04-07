using Autofac;
using System.Windows.Input;
using WPFClient.Command;
using WPFClient.Factory;

namespace WPFClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly CommandFactory commandFactory;

        public LoginViewModel(CommandFactory factory)
        {
            this.commandFactory = factory;
        }

        #region Props
        private string email = "";
        public string Email {
            get {
                return email;
            }
            set { SetField(ref email, value); }
            /*set {
                email = value;
                //TODO validation
                errors.Remove(nameof(Email));
                if (string.IsNullOrEmpty(email))
                {
                    errors.Add(nameof(Email), new List<string> { "Email empty" });
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Email)));
                }
                PropertyChanged?.Invoke(this, new(nameof(Email)));
            }*/
        }

        private string password = "";
        public string Password {
            get {
                return password;
            }
            set { SetField(ref password, value); }
        }

        private bool isLoading = false;
        public bool IsLoading {
            get { return isLoading; }
            set { SetField(ref isLoading, value); }
        }
        
        #endregion

        public ICommand LoginCommand {
            get {
                return commandFactory.Get<LoginCommand>(this);
            }
        }

        public ICommand GoRegister {
            get {
                //TODO registration
                return commandFactory.Get<LoginCommand>(this);
            }
        }
    }
}
