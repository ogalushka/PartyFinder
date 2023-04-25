using System;
using System.Windows.Input;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Command
{
    public class LogOutCommand : ICommand
    {
        private readonly Navigation navigationStore;
        private readonly PersistentStorage persistentStorage;

        public event EventHandler? CanExecuteChanged;

        public LogOutCommand(Navigation navigationStore, PersistentStorage persistentStorage)
        {
            this.navigationStore = navigationStore;
            this.persistentStorage = persistentStorage;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            persistentStorage.ClearCookies();
            navigationStore.SetViewModel<LoginViewModel>();
        }
    }
}
