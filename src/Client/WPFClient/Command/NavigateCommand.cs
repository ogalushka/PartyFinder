using System;
using System.Windows.Input;
using WPFClient.Store;

namespace WPFClient.Command
{
    public class NavigateCommand<TViewModel> : ICommand where TViewModel : ViewModelBase
    {
        private readonly Navigation navigation;

        public NavigateCommand(Navigation navigation)
        {
            this.navigation = navigation;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            navigation.SetViewModel<TViewModel>();
        }
    }
}
