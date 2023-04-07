using System;
using System.Windows.Input;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Command
{
    public class LogOutCommand : ICommand
    {
        private readonly Navigation navigationStore;

        public event EventHandler? CanExecuteChanged;

        public LogOutCommand(Navigation navigationStore)
        {
            this.navigationStore = navigationStore;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            navigationStore.SetViewModel<LoginViewModel>();
        }
    }
}
