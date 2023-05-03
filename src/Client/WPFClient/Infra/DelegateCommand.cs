using Autofac.Core;
using System;
using System.Diagnostics;
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

    public class DelegateCommand<Param> : ICommand
    {
        private readonly Action<Param> executeAction;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action<Param> executeAction, Func<bool>? canExecute = null)
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
            if (parameter is Param typed)
            {
                executeAction.Invoke(typed);
            }
            else
            {
                var actualParam = parameter != null ? parameter.GetType().ToString() : "null";
                Debug.Assert(false, $"Command executed with unsupported typeparam actual {actualParam} expected type {nameof(Param)}");
            }
        }

        private bool DefaultCanExecute()
        {
            return true;
        }
    }
}
