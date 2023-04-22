using System;
using System.Windows.Input;

namespace WPFClient.Infra
{
    public class DelegateCommand : ICommand
    {
        private readonly Action executeAction;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action executeAction, Func<bool>? canExecute = null)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute ?? DefaultCanExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute.Invoke();
        }

        public void Execute(object? parameter)
        {
            executeAction.Invoke();
        }

        public void InvokeCanExecuteChanged(EventArgs? args = null)
        {
            CanExecuteChanged?.Invoke(this, args ?? EventArgs.Empty);
        }

        private bool DefaultCanExecute()
        {
            return true;
        }
    }
}
